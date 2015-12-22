using System.Net;
using System.Web;
using System.Web.Mvc;
using OrderezeImageTask.DataAccessLayer;
using OrderezeImageTask.Models;

namespace OrderezeImageTask.Controllers
{
    public class ImageController : Controller
    {
        private ImageService _imageService = new ImageService();

        //Search image
        public ViewResult Index(string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }

            ViewBag.CurrentFilter = searchString;

            var images = _imageService.SearchImage(searchString);

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            // return View(images.ToPagedList(pageNumber, pageSize));
            return View(images);
        }


        // GET: Image/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = _imageService.FindImage(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // GET: Image/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Image/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description,ImagePath")] Image image, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                _imageService.AddNewImage(image, file);
                return RedirectToAction("Index");
            }

            return View(image);
        }

        //GET: Image/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = _imageService.FindImage(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // POST: Image/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,ImagePath")] Image image, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                _imageService.EditImage(image, file);
                return RedirectToAction("Index");
            }
            return View(image);
        }

        // GET: Image/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Image image = _imageService.FindImage(id);
            if (image == null)
            {
                return HttpNotFound();
            }
            return View(image);
        }

        // POST: Image/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            _imageService.DeleteImage(id);
            return RedirectToAction("Index");
        }

        // GET: Image
        //public ActionResult Index()  
        //{
        //    return View(_imageService.GetImages());
        //}
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

    }
}
