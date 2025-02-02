/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See license-pdn.txt for full licensing and attribution details.             //
//                                                                             //
// Ported to Pinta by: Krzysztof Marecki <marecki.krzysztof@gmail.com>         //
/////////////////////////////////////////////////////////////////////////////////

using System;
using Cairo;
using Pinta.Core;
using Pinta.Gui.Widgets;

namespace Pinta.Effects;

public sealed class EdgeDetectEffect : ColorDifferenceEffect
{
	public override string Icon => Pinta.Resources.Icons.EffectsStylizeEdgeDetect;

	public sealed override bool IsTileable => true;

	public override string Name => Translations.GetString ("Edge Detect");

	public override bool IsConfigurable => true;

	public override string EffectMenuCategory => Translations.GetString ("Stylize");

	public EdgeDetectData Data => (EdgeDetectData) EffectData!;  // NRT - Set in constructor

	private readonly IChromeService chrome;

	public EdgeDetectEffect (IServiceManager services)
	{
		chrome = services.GetService<IChromeService> ();
		EffectData = new EdgeDetectData ();
	}

	public override void LaunchConfiguration ()
	{
		chrome.LaunchSimpleEffectDialog (this);
	}

	public override void Render (ImageSurface src, ImageSurface dest, ReadOnlySpan<RectangleI> rois)
	{
		var weights = ComputeWeights ();
		base.RenderColorDifferenceEffect (weights, src, dest, rois);
	}

	private double[][] ComputeWeights ()
	{
		var weights = new double[3][];
		for (int i = 0; i < weights.Length; ++i) {
			weights[i] = new double[3];
		}

		// adjust and convert angle to radians
		double r = (double) Data.Angle.Degrees * 2.0 * Math.PI / 360.0;

		// angle delta for each weight
		double dr = Math.PI / 4.0;

		// for r = 0 this builds an edge detect filter pointing straight left

		weights[0][0] = Math.Cos (r + dr);
		weights[0][1] = Math.Cos (r + 2.0 * dr);
		weights[0][2] = Math.Cos (r + 3.0 * dr);

		weights[1][0] = Math.Cos (r);
		weights[1][1] = 0;
		weights[1][2] = Math.Cos (r + 4.0 * dr);

		weights[2][0] = Math.Cos (r - dr);
		weights[2][1] = Math.Cos (r - 2.0 * dr);
		weights[2][2] = Math.Cos (r - 3.0 * dr);

		return weights;
	}
}

public sealed class EdgeDetectData : EffectData
{
	[Caption ("Angle")]
	public DegreesAngle Angle { get; set; } = new (45);
}
