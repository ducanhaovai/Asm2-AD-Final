using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq.Expressions;
using Tranning.DataDBContext;
using Tranning.Models;

namespace Tranning.Controllers
{
    public class RoleController : Controller
    {
        private readonly TranningDBContext _dbContext;
        public RoleController(TranningDBContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        public IActionResult Index(string SearchString)
        {

            RoleModel roleModel = new RoleModel();
            roleModel.RoleDetailLists = new List<RoleDetail>();

            var data = from m in _dbContext.Roles
                       select m;

            data = data.Where(m => m.deleted_at == null);
            if (!string.IsNullOrEmpty(SearchString))
            {
                data = data.Where(m => m.name.Contains(SearchString) || m.description.Contains(SearchString));
            }
            data.ToList();

            foreach (var item in data)
            {
                roleModel.RoleDetailLists.Add(new RoleDetail
                {
                    id = item.id,
                    name = item.name,
                    description = item.description,
                    
                    status = item.status,
                    created_at = item.created_at,
                    updated_at = item.updated_at
                });
            }
            ViewData["CurrentFilter"] = SearchString;
            return View(roleModel);
        }

        [HttpGet]
        public IActionResult Add()
        {
           
            RoleDetail role = new RoleDetail();
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RoleDetail role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    var roleData = new Role()
                    {
                        name = role.name,
                        description = role.description,
                        status = role.status,
                        created_at = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    };
                    _dbContext.Roles.Add(roleData);
                    _dbContext.SaveChanges(true);
                    TempData["saveStatus"] = true;
                } 
                catch
                {
                    TempData["saveStatus"] = false;
                }
                return RedirectToAction(nameof(RoleController.Index), "Role");
            }
            return View(role);
        }

        [HttpGet]
        public IActionResult Update(int id = 0)
        {
            RoleDetail role = new RoleDetail();
            var data = _dbContext.Roles.Where(m => m.id == id).FirstOrDefault();
            if (data != null)
            {
                role.id = data.id;
                role.name = data.name;
                role.description = data.description;
                
                role.status = data.status;
            }

            return View(role);
        }

        [HttpPost]
        public IActionResult Update(RoleDetail role, IFormFile file)
        {
            try
            {

                var data = _dbContext.Roles.Where(m => m.id == role.id).FirstOrDefault();
                

                if (data != null)
                {
                    // gan lai du lieu trong db bang du lieu tu form model gui len
                    data.name = role.name;
                    data.description = role.description;
                    data.status = role.status;
                    
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
            return RedirectToAction(nameof(RoleController.Index), "Role");
        }

        [HttpGet]
        public IActionResult Delete(int id = 0)
        {
            try
            {
                var data = _dbContext.Roles.Where(m => m.id == id).FirstOrDefault();
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
            return RedirectToAction(nameof(RoleController.Index), "Role");
        }

        
    }
}
