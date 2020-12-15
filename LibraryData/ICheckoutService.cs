using System.Collections.Generic;
using LibraryData.Models;
using System;

namespace LibraryData
{
    public interface ICheckoutService
    {
        void Add(Checkout newCheckout);

        IEnumerable<Checkout> GetAll();
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int id);
        IEnumerable<Hold> GetCurrentHolds(int id);

        Checkout GetById(int checkoutId);
        Checkout GetLatestCheckout(int assetId);
        string GetCurrentCheckoutPatron(int assetId);
        string GetCurrentHoldPatronName(int id);
        DateTime GetCurrentHoldPlaced(int id);
        bool IsCheckedOut(int id);

        void CheckoutItem(int assetId, int libraryCardId);
        void CheckInItem(int assetId);
        void PlaceHold(int assetId, int libraryCardId);  
        void MarkLost(int assetId);
        void MarkFound(int assetId);

        //int GetNumberOfCopies(int id);

        //DateTime GetCurrentPatron(int id);


    }
}