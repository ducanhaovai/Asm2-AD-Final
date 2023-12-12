using Microsoft.AspNetCore.Mvc;
using Tranning.DataDBContext;
using Tranning.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

public class TopicController : Controller
{
    private readonly TranningDBContext _dbContext;

    public TopicController(TranningDBContext context)
    {
        _dbContext = context;
    }

    [HttpGet]
    public IActionResult Index(string SearchString)
    {
        TopicModel topicModel = new TopicModel();
        topicModel.TopicDetailLists = new List<TopicDetail>();

            var data = from m in _dbContext.Topics
                       select m;

            data = data.Where(m => m.deleted_at == null);
            if (!string.IsNullOrEmpty(SearchString))
            {

            data = data.Where(m => m.name.Contains(SearchString) || m.description.Contains(SearchString));
            }
            data.ToList();

            foreach (var item in data)
            {
                topicModel.TopicDetailLists.Add(new TopicDetail
                {
                    id = item.id,
                    course_id = item.course_id,
                    
                    name = item.name,
                    description = item.description,
                    videos = item.videos,
                    attach_file = item.attach_file,
                    status = item.status,

                    created_at = item.created_at,
                    updated_at = item.updated_at,
                    deleted_at = item.deleted_at
                });
            }
        
        

        ViewData["CurrentFilter"] = SearchString;
        return View(topicModel);
    }

    [HttpGet]
    public IActionResult Add()
    {
        ViewBag.Stores = GetCoursesForDropdown();

        TopicDetail topic = new TopicDetail();
        return View(topic);
    }

    private List<SelectListItem> GetCoursesForDropdown()
    {
        // Assuming you have a Courses DbSet in your DbContext
        var courses = _dbContext.Courses
            .Where(c => c.deleted_at == null) // Adjust this condition based on your business logic
            .Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.name
            })
            .ToList();

        return courses;
    }
    private bool IsValidUrl(string url)
    {
        Uri uriResult;
        return Uri.TryCreate(url, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(TopicDetail topic, IFormFile Photo)
    {
        if (!ModelState.IsValid)
        {
           
            try
            {

                string uniqueAttachFile = UploadFile(Photo);


                var topicData = new Topic()
                {
                    course_id = topic.course_id,
                    name = topic.name,
                    description = topic.description,
                    videos = topic.youtubeVideoId,
                    attach_file = uniqueAttachFile,
                    status = topic.status,

                    created_at = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                };
                _dbContext.Topics.Add(topicData);
                _dbContext.SaveChanges(true);
                TempData["saveStatus"] = true;
            }
            catch (Exception ex)
            {
                TempData["saveStatus"] = false;
            }
            return RedirectToAction(nameof(TopicController.Index), "Topic");
        }
        var courses = GetCoursesForDropdown();
        Debug.WriteLine($"Courses count: {courses.Count}");
        ViewBag.Stores = courses;


        return View(topic);
    }

    // Other actions (Update, Delete, etc.) follow the same pattern as in CourseController

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
    public IActionResult Update(int id = 0)
    {
        TopicDetail topic = new TopicDetail();
        var data = _dbContext.Topics.Where(m => m.id == id).FirstOrDefault();
        if (data != null)
        {
            topic.id = data.id;
            topic.name = data.name;
            topic.course_id = data.course_id;
            topic.description = data.description;
            topic.videos = data.videos;
            topic.attach_file = data.attach_file;
            topic.status = data.status;
         
            
            
        }
        var courses = GetCoursesForDropdown();
        Debug.WriteLine($"Courses count: {courses.Count}");
        ViewBag.Stores = courses;


        return View(topic);

    }

    [HttpPost]
    public IActionResult Update(TopicDetail topic, IFormFile file)
    {
        try
        {
            var data = _dbContext.Topics.Where(m => m.id == topic.id).FirstOrDefault();
            string uniqueAttachFileName = "";
            if (file != null)
            {
                uniqueAttachFileName = UploadFile(file);
            }

            if (data != null)
            {
                data.name = topic.name;
                data.course_id = topic.course_id;
                data.description = topic.description;
                data.videos = topic.youtubeVideoId;

                if (!string.IsNullOrEmpty(uniqueAttachFileName))
                {
                    data.attach_file = uniqueAttachFileName;
                }

                data.status = topic.status;
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
        return RedirectToAction(nameof(TopicController.Index), "Topic");
    }

    [HttpGet]
    public IActionResult Delete(int id = 0)
    {
        try
        {
            var data = _dbContext.Topics.Where(m => m.id == id).FirstOrDefault();
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
        return RedirectToAction(nameof(CourseController.Index), "Topic");
    }
}
