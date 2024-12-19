using System;
using Android.Runtime;

namespace AndroidX.Transitions
{
}


namespace AndroidX.Transitions
{

	// Metadata.xml XPath class reference: path="/api/package[@name='androidx.transition']/class[@name='FloatArrayEvaluator']"
	//[global::Android.Runtime.Register("androidx/transition/FloatArrayEvaluator", DoNotGenerateAcw = true)]
	public partial class FloatArrayEvaluator : global::Java.Lang.Object, global::Android.Animation.ITypeEvaluator
	{
		// Metadata.xml XPath method reference: path="/api/package[@name='androidx.transition']/class[@name='FloatArrayEvaluator']/method[@name='evaluate' and count(parameter)=3 and parameter[1][@type='float'] and parameter[2][@type='float[]'] and parameter[3][@type='float[]']]"
		//[Register("evaluate", "(F[F[F)[F", "GetEvaluate_FarrayFarrayFHandler")]
		public virtual unsafe Java.Lang.Object Evaluate(float fraction, Java.Lang.Object startValue, Java.Lang.Object endValue)
		{
			return Evaluate(fraction, (float[]) startValue, (float) endValue);
		}
	}

	// Metadata.xml XPath class reference: path="/api/package[@name='androidx.transition']/class[@name='TransitionUtils']"
	//[global::Android.Runtime.Register("androidx/transition/TransitionUtils", DoNotGenerateAcw = true)]
	public partial class TransitionUtils : global::Java.Lang.Object
	{
		// Metadata.xml XPath class reference: path="/api/package[@name='androidx.transition']/class[@name='TransitionUtils.MatrixEvaluator']"
		//[global::Android.Runtime.Register("androidx/transition/TransitionUtils$MatrixEvaluator", DoNotGenerateAcw = true)]
		public partial class MatrixEvaluator : global::Java.Lang.Object, global::Android.Animation.ITypeEvaluator
		{
			public virtual unsafe Java.Lang.Object Evaluate(float fraction, Java.Lang.Object startValue, Java.Lang.Object endValue)
			{
				return Evaluate(fraction, (float[])startValue, (float)endValue);
			}
		}
	}

	// Metadata.xml XPath class reference: path="/api/package[@name='androidx.transition']/class[@name='RectEvaluator']"
	//[global::Android.Runtime.Register("androidx/transition/RectEvaluator", DoNotGenerateAcw = true)]
	public partial class RectEvaluator : global::Java.Lang.Object, global::Android.Animation.ITypeEvaluator
	{
		public virtual unsafe Java.Lang.Object Evaluate(float fraction, Java.Lang.Object startValue, Java.Lang.Object endValue)
		{
			return Evaluate(fraction, (float[])startValue, (float)endValue);
		}
	}

}