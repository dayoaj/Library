using System.Linq;
using LibraryData;
using Library.Models.Catalog;
using Microsoft.AspNetCore.Mvc;
using Library.Models.CheckoutModels;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private ILibraryAssetService _assetsService;
        private ICheckoutService _checkoutsService;

        public CatalogController(ILibraryAssetService assets, ICheckoutService checkouts)
        {
            _assetsService = assets;
            _checkoutsService = checkouts;
        }

        public IActionResult Index()
        {
            var assetModels = _assetsService.GetAll();

            var listingResult = assetModels.Select( result => new AssetIndexListingModel {
                Id = result.Id,
                ImageUrl = result.ImageUrl,
                AuthorOrDirector = _assetsService.GetAuthorOrDirector(result.Id),
                DeweyCallNumber = _assetsService.GetDeweyIndex(result.Id),
                Title = result.Title,
                Type = _assetsService.GetType(result.Id)
            }).ToList();

            var model = new AssetIndexModel() {
                Assets = listingResult
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var asset = _assetsService.Get(id);

            var currentHolds = _checkoutsService.GetCurrentHolds(id).Select(a => new AssetHoldModel
            {
                HoldPlaced = _checkoutsService.GetCurrentHoldPlaced(a.Id).ToString("d"),
                PatronName = _checkoutsService.GetCurrentHoldPatronName(a.Id)
            });

            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Type = _assetsService.GetType(id),
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthorOrDirector = _assetsService.GetAuthorOrDirector(id),
                CurrentLocation = _assetsService.GetCurrentLocation(id)?.Name,
                DeweyCallNumber = _assetsService.GetDeweyIndex(id),
                CheckoutHistory = _checkoutsService.GetCheckoutHistory(id),
                CurrentAssociatedLibraryCard = _assetsService.GetLibraryCardByAssetId(id),
                ISBN = _assetsService.GetIsbn(id),
                LatestCheckout = _checkoutsService.GetLatestCheckout(id),
                CurrentHolds = currentHolds,
                PatronName = _checkoutsService.GetCurrentCheckoutPatron(id)
            };

            return View(model);
        }

        public IActionResult Checkout(int id)
        {
            var asset = _assetsService.Get(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkoutsService.IsCheckedOut(id)
            };
            return View(model);
        }

        public IActionResult CheckIn(int id)
        {
            _checkoutsService.CheckInItem(id);
            return RedirectToAction("Detail", new { id });
        }

        public IActionResult Hold(int id)
        {
            var asset = _assetsService.Get(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                HoldCount = _checkoutsService.GetCurrentHolds(id).Count()
            };
            return View(model);
        }

        public IActionResult MarkLost(int id)
        {
            _checkoutsService.MarkLost(id);
            return RedirectToAction("Detail", new { id });
        }

        public IActionResult MarkFound(int id)
        {
            _checkoutsService.MarkFound(id);
            return RedirectToAction("Detail", new { id });
        }

        [HttpPost]
        public IActionResult PlaceCheckout(int assetId, int libraryCardId)
        {
            _checkoutsService.CheckoutItem(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId, int libraryCardId)
        {
            _checkoutsService.PlaceHold(assetId, libraryCardId);
            return RedirectToAction("Detail", new { id = assetId });
        }
    }
}