using System.Collections.Generic;

namespace Xamarin.Grpc;

public partial class LongGaugeMetricInstrument
{
    IList<string>? IMetricInstrument.OptionalLabelKeys => (IList<string>?) OptionalLabelKeys;

    IList<string>? IMetricInstrument.RequiredLabelKeys => (IList<string>?) RequiredLabelKeys;
}
