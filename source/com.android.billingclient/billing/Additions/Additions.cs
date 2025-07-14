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

    [Obsolete("QueryPurchaseHistory was removed in Billing Client v8.0.0. Use QueryPurchasesAsync instead.")]
    public class QueryPurchaseHistoryResult
    {
        public BillingResult Result { get; set; }

        public IList<PurchaseHistoryRecord> PurchaseHistoryRecords { get; set; }
    }

    [Obsolete("Use QueryProductDetailsAsync(QueryProductDetailsParams) instead")]
    public class QuerySkuDetailsResult
    {
        public BillingResult Result { get; set; }

        public IList<SkuDetails> SkuDetails { get; set; }
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

        const string QueryPurchaseHistoryNotSupported = "QueryPurchaseHistory method was removed in Billing Client v8.0.0. Use QueryPurchasesAsync instead. See: https://developer.android.com/google/play/billing/migrate";

        [Obsolete(QueryPurchaseHistoryNotSupported, error: true)]
        public Task<QueryPurchaseHistoryResult> QueryPurchaseHistoryAsync(string skuType)
        {
            // Migration: QueryPurchaseHistory was replaced by QueryPurchases
            // However, we cannot provide automatic migration as the parameters and return types are different
            throw new NotSupportedException(QueryPurchaseHistoryNotSupported);
        }

        [Obsolete(QueryPurchaseHistoryNotSupported, error: true)]
        public Task<QueryPurchaseHistoryResult> QueryPurchaseHistoryAsync(QueryPurchaseHistoryParams queryPurchaseHistoryParams)
        {
            // Migration: QueryPurchaseHistory was replaced by QueryPurchases
            // However, we cannot provide automatic migration as the parameters and return types are different
            throw new NotSupportedException(QueryPurchaseHistoryNotSupported);
        }

        const string QuerySkuDetailsNotSupported = "QuerySkuDetails method was removed in Billing Client v8.0.0. Use QueryProductDetailsAsync instead. See: https://developer.android.com/google/play/billing/migrate";

        [Obsolete(QuerySkuDetailsNotSupported, error: true)]
        public Task<QuerySkuDetailsResult> QuerySkuDetailsAsync(SkuDetailsParams skuDetailsParams)
        {
            // Migration: QuerySkuDetails was replaced by QueryProductDetails
            // However, we cannot provide automatic migration as the parameters and return types are different
            throw new NotSupportedException(QuerySkuDetailsNotSupported);
        }

        public Task<QueryProductDetailsResult> QueryProductDetailsAsync(QueryProductDetailsParams productDetailsParams)
        {
            var tcs = new TaskCompletionSource<QueryProductDetailsResult>();

            var listener = new InternalProductDetailsResponseListener
            {
                ProductDetailsResponseHandler = (r, queryResult) => tcs.TrySetResult(queryResult)
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

    internal class InternalPriceChangeConfirmationListener : Java.Lang.Object //, IPriceChangeConfirmationListener
    {
        public Action<BillingResult> PriceChangeConfirmationHandler { get; set; }
        public void OnPriceChangeConfirmationResult(BillingResult result)
            => PriceChangeConfirmationHandler?.Invoke(result);
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

    [Obsolete("Use QueryProductDetailsAsync(QueryProductDetailsParams) instead")]
    internal class InternalSkuDetailsResponseListener : Java.Lang.Object, ISkuDetailsResponseListener
    {
        public Action<BillingResult, IList<SkuDetails>> SkuDetailsResponseHandler { get; set; }

        public void OnSkuDetailsResponse(BillingResult result, IList<SkuDetails> skuDetails)
            => SkuDetailsResponseHandler?.Invoke(result, skuDetails);
    }

    internal class InternalProductDetailsResponseListener : Java.Lang.Object, IProductDetailsResponseListener
    {
        public Action<BillingResult, QueryProductDetailsResult> ProductDetailsResponseHandler { get; set; }

        public void OnProductDetailsResponse(BillingResult result, QueryProductDetailsResult queryResult)
            => ProductDetailsResponseHandler?.Invoke(result, queryResult);
    }

    internal class InternalPurchasesResponseListener : Java.Lang.Object, IPurchasesResponseListener
    {
        public Action<BillingResult, IList<Purchase>> PurchasesResponseHandler { get; set; }

        public void OnQueryPurchasesResponse(BillingResult result, IList<Purchase> purchases)
            => PurchasesResponseHandler?.Invoke(result, purchases);
    }
}
