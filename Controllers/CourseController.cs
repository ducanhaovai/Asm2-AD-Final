using Microsoft.AspNetCore.Mvc;
using Tranning.DataDBContext;
using Tranning.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

public class CourseController : Controller
{
    private readonly TranningDBContext _dbContext;

    public CourseController(TranningDBContext context)
    {
        _dbContext = context;
    }

    [HttpGet]
    public IActionResult Index(string SearchString)
    {
        CourseModel courseModel = new CourseModel();
        courseModel.CourseDetailLists = new List<CourseDetail>();

        
            var data = from m in _dbContext.Courses
                       select m;

            data = data.Where(m => m.deleted_at == null);
            if (!string.IsNullOrEmpty(SearchString))
            {
                data = data.Where(m => m.name.Contains(SearchString) || m.description.Contains(SearchString));
            }
            data.ToList();


            foreach (var item in data)
            {
                courseModel.CourseDetailLists.Add(new CourseDetail
                {
                    id = item.id,
                    name = item.name,
                    category_id = item.category_id,
                    description = item.description,
                    start_date = item.start_date,
                    end_date = item.end_date,

                    avatar = item.avatar,
                    status = item.status,
                    created_at = item.created_at,
                    updated_at = item.updated_at
                });
            }
        
        //return Ok(courseModel.CourseDetailLists);
        ViewData["CurrentFilter"] = SearchString;
        return View(courseModel);
    }

    [HttpGet]
    public IActionResult Add()
    {
        ViewBag.Stores = GetCategoriesForDropdown();

        CourseDetail course = new CourseDetail();
        return View(course);
    }
    private List<SelectListItem> GetCategoriesForDropdown()
    {
        // Assuming you have a Categories DbSet in your DbContext
        var categories = _dbContext.Categories
            .Where(c => c.deleted_at == null) // Adjust this condition based on your business logic
            .Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.name
            })
            .ToList();

        return categories;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(CourseDetail course, IFormFile Photo)
    {
        if (ModelState.IsValid)
        {
            
            try
            {
                string uniqueFileName = UploadFile(Photo);
                var courseData = new Course()
                {
                    name = course.name,
                    description = course.description,
                    category_id = course.category_id,
                    start_date = course.start_date,
                    end_date = course.end_date,

                    avatar = uniqueFileName,
                    status = course.status,
                    created_at = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                };
                _dbContext.Courses.Add(courseData);
                _dbContext.SaveChanges(true);
                TempData["saveStatus"] = true;
                
            }
            catch (Exception ex)
            {

                TempData["saveStatus"] = false;
                
            }
            return RedirectToAction(nameof(CourseController.Index), "Course");
        }

        var categories = GetCategoriesForDropdown();
        Debug.WriteLine($"Categories count: {categories.Count}");
        // Repopulate ViewBag.Stores with categories before returning the view
        ViewBag.Stores = categories;

        return View(course);
    }

    [HttpGet]
    public IActionResult Update(int id = 0)
    {
        CourseDetail course = new CourseDetail();
        var data = _dbContext.Courses.Where(m => m.id == id).FirstOrDefault();
        if (data != null)
        {
            course.id = data.id;
            course.name = data.name;
            course.category_id = data.category_id;
            course.description = data.description;
            course.start_date = data.start_date;
            course.end_date = data.end_date;
            course.vote = data.vote;
            course.avatar = data.avatar;
            course.status = data.status;
        }
        var categories = GetCategoriesForDropdown();
        Debug.WriteLine($"Categories count: {categories.Count}");
        // Repopulate ViewBag.Stores with categories before returning the view
        ViewBag.Stores = categories;

        return View(course);
        
    }

    [HttpPost]
    public IActionResult Update(CourseDetail course, IFormFile file)
    {
        try
        {
            var data = _dbContext.Courses.Where(m => m.id == course.id).FirstOrDefault();
            string uniqueAvatarFileName = "";
            if (course.Photo != null)
            {
                uniqueAvatarFileName = UploadFile(course.Photo);
            }

            if (data != null)
            {
                data.name = course.name;
                data.category_id = course.category_id;
                
                data.description = course.description;
                data.start_date = course.start_date;
                data.end_date = course.end_date;
                data.vote = course.vote;
                if (!string.IsNullOrEmpty(uniqueAvatarFileName))
                {
                    data.avatar = uniqueAvatarFileName;
                }
                data.status = course.status;
                _dbContext.SaveChanges(true);
                TempData["UpdateStatus"] = true;
            }
            else
            {
                TempData["UpdateStatus"] = false;
            }
        }
        catch (Exception ex)
        {
            TempData["UpdateStatus"] = false;
        }
        return RedirectToAction(nameof(CourseController.Index), "Course");
    }

    [HttpGet]
    public IActionResult Delete(int id = 0)
    {
        try
        {
            var data = _dbContext.Courses.Where(m => m.id == id).FirstOrDefault();
            if (data != null)
            {
                data.deleted_at = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                _dbContext.SaveChanges(true);
                TempData["DeleteStatus"] = true;
            }
            else
            {
                TempData["DeleteStatus"] = false;
            }
        }
        catch
        {
            TempData["DeleteStatus"] = false;
        }
        return RedirectToAction(nameof(CourseController.Index), "Course");
    }

    private string UploadFile(IFormFile file)
    {
        string uniqueFileName;
        try
        {
            string pathUploadServer = "wwwroot\\uploads\\images";

            string fileName = file.FileName;
            fileName = Path.GetFileName(fileName);
            string uniqueStr = Guid.NewGuid().ToString();
            fileName = uniqueStr + "-" + fileName;
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), pathUploadServer, fileName);
            var stream = new FileStream(uploadPath, FileMode.Create);
            file.CopyToAsync(stream);
            uniqueFileName = fileName;
        }
        catch (Exception ex)
        {
            uniqueFileName = ex.Message.ToString();
        }
        return uniqueFileName;
    }
}
