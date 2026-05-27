using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Android.BillingClient.Api
{
    public class ConsumeResult
    {
        public BillingResult BillingResult { get; set; }

        public string PurchaseToken { get; set; }
    }

    public partial class QueryProductDetailsResult
    {
        public QueryProductDetailsResult() { }

        public BillingResult Result { get; set; }

        public IList<ProductDetails> ProductDetails { get; set; }
    }

    public class QueryPurchasesResult
    {
        public BillingResult Result { get; set; }

        public IList<Purchase> Purchases { get; set; }
    }

    public partial class BillingClient
    {
        public partial class Builder
        {
            InternalPurchasesUpdatedListener purchasesUpdatedListener;

            public void SetListener(Action<BillingResult, IList<Purchase>> handler)
            {
                purchasesUpdatedListener = new InternalPurchasesUpdatedListener
                {
                    PurchasesUpdatedHandler = (r, p) => handler?.Invoke(r, p)
                };

                SetListener(purchasesUpdatedListener);
            }
        }

        public Task<BillingResult> AcknowledgePurchaseAsync(AcknowledgePurchaseParams acknowledgePurchaseParams)
        {
            var tcs = new TaskCompletionSource<BillingResult>();

            var listener = new InternalAcknowledgePurchaseResponseListener
            {
                AcknowledgePurchaseResponseHandler = r => tcs.TrySetResult(r)
            };

            AcknowledgePurchase(acknowledgePurchaseParams, listener);

            return tcs.Task;
        }

        public Task<ConsumeResult> ConsumeAsync(ConsumeParams consumeParams)
        {
            var tcs = new TaskCompletionSource<ConsumeResult>();

            var listener = new InternalConsumeResponseListener
            {
                ConsumeResponseHandler = (r, s) => tcs.TrySetResult(new ConsumeResult
                {
                    BillingResult = r,
                    PurchaseToken = s
                })
            };

            Consume(consumeParams, listener);

            return tcs.Task;
        }

        public Task<QueryProductDetailsResult> QueryProductDetailsAsync(QueryProductDetailsParams productDetailsParams)
        {
            var tcs = new TaskCompletionSource<QueryProductDetailsResult>();

            // NOTE: this creates a new QueryProductDetailsResult to avoid ObjectDisposedException
            var listener = new InternalProductDetailsResponseListener
            {
                ProductDetailsResponseHandler = (r, s) => tcs.TrySetResult(new QueryProductDetailsResult
                {
                    Result = r,
                    ProductDetails = s
                })
            };

            QueryProductDetails(productDetailsParams, listener);

            return tcs.Task;
        }

        public Task<QueryPurchasesResult> QueryPurchasesAsync(QueryPurchasesParams purchasesParams)
        {
            var tcs = new TaskCompletionSource<QueryPurchasesResult>();

            var listener = new InternalPurchasesResponseListener
            {
                PurchasesResponseHandler = (r, s) => tcs.TrySetResult(new QueryPurchasesResult
                {
                    Result = r,
                    Purchases = s
                })
            };

            QueryPurchases(purchasesParams, listener);

            return tcs.Task;
        }

        public Task<BillingResult> StartConnectionAsync(Action onDisconnected = null)
        {
            var tcs = new TaskCompletionSource<BillingResult>();

            var listener = new InternalBillingClientStateListener
            {
                BillingServiceDisconnectedHandler = () =>
                {
                    onDisconnected?.Invoke();
                    tcs.TrySetResult(null);
                },
                BillingSetupFinishedHandler = r =>
                    tcs.TrySetResult(r)
            };

            StartConnection(listener);

            return tcs.Task;
        }

        public void StartConnection(Action<BillingResult> setupFinished, Action onDisconnected)
        {
            var listener = new InternalBillingClientStateListener
            {
                BillingServiceDisconnectedHandler = () =>
                    onDisconnected?.Invoke(),
                BillingSetupFinishedHandler = r =>
                    setupFinished?.Invoke(r)
            };

            StartConnection(listener);
        }
    }

    internal class InternalAcknowledgePurchaseResponseListener : Java.Lang.Object, IAcknowledgePurchaseResponseListener
    {
        public Action<BillingResult> AcknowledgePurchaseResponseHandler { get; set; }

        public void OnAcknowledgePurchaseResponse(BillingResult result)
            => AcknowledgePurchaseResponseHandler?.Invoke(result);
    }

    internal class InternalBillingClientStateListener : Java.Lang.Object, IBillingClientStateListener
    {
        public Action BillingServiceDisconnectedHandler { get; set; }

        public Action<BillingResult> BillingSetupFinishedHandler { get; set; }

        public void OnBillingServiceDisconnected()
            => BillingServiceDisconnectedHandler?.Invoke();

        public void OnBillingSetupFinished(BillingResult result)
            => BillingSetupFinishedHandler?.Invoke(result);
    }

    internal class InternalConsumeResponseListener : Java.Lang.Object, IConsumeResponseListener
    {
        public Action<BillingResult, string> ConsumeResponseHandler { get; set; }
        public void OnConsumeResponse(BillingResult result, string str)
            => ConsumeResponseHandler?.Invoke(result, str);
    }

    internal class InternalPurchaseHistoryResponseListener : Java.Lang.Object, IPurchaseHistoryResponseListener
    {
        public Action<BillingResult, IList<PurchaseHistoryRecord>> PurchaseHistoryResponseHandler { get; set; }

        public void OnPurchaseHistoryResponse(BillingResult result, IList<PurchaseHistoryRecord> history)
            => PurchaseHistoryResponseHandler?.Invoke(result, history);
    }

    internal class InternalPurchasesUpdatedListener : Java.Lang.Object, IPurchasesUpdatedListener
    {
        public Action<BillingResult, IList<Purchase>> PurchasesUpdatedHandler { get; set; }
        public void OnPurchasesUpdated(BillingResult result, IList<Purchase> purchases)
            => PurchasesUpdatedHandler?.Invoke(result, purchases);
    }

    internal class InternalProductDetailsResponseListener : Java.Lang.Object, IProductDetailsResponseListener
    {
        public Action<BillingResult, IList<ProductDetails>> ProductDetailsResponseHandler { get; set; }

        public void OnProductDetailsResponse(BillingResult result, QueryProductDetailsResult queryResult)
        {
            ProductDetailsResponseHandler?.Invoke(result, queryResult?.ProductDetailsList);
        }
    }

    internal class InternalPurchasesResponseListener : Java.Lang.Object, IPurchasesResponseListener
    {
        public Action<BillingResult, IList<Purchase>> PurchasesResponseHandler { get; set; }

        public void OnQueryPurchasesResponse(BillingResult result, IList<Purchase> purchases)
            => PurchasesResponseHandler?.Invoke(result, purchases);
    }
}
