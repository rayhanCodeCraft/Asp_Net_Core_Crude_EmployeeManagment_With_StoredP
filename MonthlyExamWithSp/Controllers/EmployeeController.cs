using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MonthlyExamWithSp.Models;
using MonthlyExamWithSp.Models.ViewModel;
using System.Data;

namespace MonthlyExamWithSp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EmployeeController(AppDbContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var data = _db.Employees;
           
            decimal min = 0, max = 0, sum = 0, avg = 0, count = 0;
            if (data.Any())
            {
                min = data.Min(e => e.Salary);
                max = data.Max(e => e.Salary);
                sum = data.Sum(e => e.Salary);
                avg = (decimal)data.Average(e => e.Salary);
                count = data.Count();
            }
           var groupbyresult = data.GroupBy(i => i.EmployeeId).Select(c => new GroupByViewModel
                {
                    EmployeeId = c.Key,
                    MinValue = c.Min(e => e.Salary),
                    MaxValue = c.Max(e => e.Salary),
                    SumValue = c.Sum(e => e.Salary),
                    AvgValue = Convert.ToDecimal(c.Average(e => e.Salary)),
                    Count = c.Count()
                }).ToList();

            
      
             

          
            var employees = await data
                .Include(e => e.Experiences)
                .Select(e => new EmployeeVM
                {
                    EmployeeId = e.EmployeeId,
                    Name = e.Name,
                    IsActive = e.IsActive,
                    JoinDate = e.JoinDate,
                    Salary = e.Salary,
                    ImageUrl = e.ImageUrl,
                    Experiences = e.Experiences.Select(exp => new ExperienceViewModel
                    {
                        Title = exp.Title,
                        Duration = exp.Duration
                    }).ToList()
                }).ToListAsync();

          
            var model = new AggregateEmployeeViewModel
            {
                MinValue = min,
                MaxValue = max,
                SumValue = sum,
                AvgValue = Convert.ToDecimal(avg),
                GroupByResult = groupbyresult,
                Employees = employees
            };

            return View(model);
        }



        [HttpGet]
        public IActionResult Create()
        {
            var model = new EmployeeVM();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeVM model)
        {
          
            string imageName = null;
            string imageUrl = null;

            if (model.ImageFile != null)
            {
            
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                imageName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, imageName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

                imageUrl = "/images/" + imageName;
            }

            var expTable = new DataTable();
            expTable.Columns.Add("Title", typeof(string));
            expTable.Columns.Add("Duration", typeof(int));

            foreach (var exp in model.Experiences)
            {
                expTable.Rows.Add(exp.Title, exp.Duration);
            }

            var parameters = new[]
            {
                new SqlParameter("@Name", model.Name),
                new SqlParameter("@IsActive", model.IsActive),
                new SqlParameter("@JoinDate", model.JoinDate),
                new SqlParameter("@ImageName", imageName ?? (object)DBNull.Value),
                new SqlParameter("@ImageUrl", imageUrl ?? (object)DBNull.Value),
                new SqlParameter("@Salary", model.Salary),
                new SqlParameter("@Exp", expTable) { SqlDbType = SqlDbType.Structured, TypeName = "dbo.ParamExpType" }
            };

            await _db.Database.ExecuteSqlRawAsync("EXEC InsertEmployeeSP @Name, @IsActive, @JoinDate, @ImageName, @ImageUrl, @Salary, @Exp", parameters);

            return RedirectToAction(nameof(Index));       
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _db.Employees.Include(e => e.Experiences)
                                                   .FirstOrDefaultAsync(e => e.EmployeeId == id);
            if (employee == null)
            {
                return NotFound();
            }

            var model = new EmployeeVM
            {
                EmployeeId = employee.EmployeeId,
                Name = employee.Name,
                IsActive = employee.IsActive,
                JoinDate = employee.JoinDate,
                Salary = employee.Salary,
                ImageName = employee.ImageName,
                ImageUrl = employee.ImageUrl,
                Experiences = employee.Experiences.Select(e => new ExperienceViewModel
                {
                    Title = e.Title,
                    Duration = e.Duration
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Edit(EmployeeVM model)
        {
            
            var employee = await _db.Employees.FirstOrDefaultAsync(e => e.EmployeeId == model.EmployeeId);
            if (employee == null)
            {
                return NotFound();
            }

        
            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                string imageName = Guid.NewGuid().ToString() + "_" + model.ImageFile.FileName;
                string filePath = Path.Combine(uploadsFolder, imageName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }

  
                if (!string.IsNullOrEmpty(employee.ImageName))
                {
                    string oldImagePath = Path.Combine(uploadsFolder, employee.ImageName);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                employee.ImageName = imageName;
                employee.ImageUrl = "/images/" + imageName;
            }
            employee.Name = model.Name;
            employee.IsActive = model.IsActive;
            employee.JoinDate = model.JoinDate;
            employee.Salary = model.Salary;

            var exe = _db.Experiences.Where(e => e.EmployeeId == model.EmployeeId).ToList();
            _db.Experiences.RemoveRange(exe);

            foreach (var exp in model.Experiences)
            {
                Experience experience = new Experience()
                {
                    Title = exp.Title,
                    Duration = exp.Duration,
                    EmployeeId = employee.EmployeeId 
                };
                _db.Experiences.Add(experience);

            }        
            await _db.SaveChangesAsync();
           return RedirectToAction(nameof(Index));
        }

        

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }         
            _db.Employees.Remove(employee);           
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
