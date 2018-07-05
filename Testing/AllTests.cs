using System;
using JsonSettings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Models;

namespace Testing
{
	[TestClass]
	public class AllTests
	{
		[TestMethod]
		public void TestMethod1()
		{
			var settings = JsonSettingsBase.Load<Settings>("~\\MySettings.json");
		}
	}
}
