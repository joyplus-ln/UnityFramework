//using UnityEngine;
//using UnityEngine.TestTools;
//using NUnit.Framework;
//using System.Collections;

//public class TreasureChestTest {

//	[Test]
//	public void TreasureChestTestSimplePasses() {
//		// Use the Assert class to test conditions.
//	}

//	[Test]
//	public void HandleChest() {
//		TreasureChestCommandGroup treasureChestCommandGroup = new TreasureChestCommandGroup(null, null, null, null);
//		bool result = treasureChestCommandGroup.HandleChests(null,null);
//		Assert.IsFalse(result);
//		RepChestData chestData = new RepChestData();
//		chestData.type = TreasureChestEnum.TypeChest;
//		RepChestData.RepGoodsData repGoodsData = new RepChestData.RepGoodsData();
//		repGoodsData.priority = 10;
//		RepChestData.RepGoodsData repGoodsData1 = new RepChestData.RepGoodsData();
//		repGoodsData1.priority = 12;
//		chestData.info = new System.Collections.Generic.List<RepChestData.RepGoodsData>();
//		result = treasureChestCommandGroup.HandleChests(chestData,null);
//		Assert.IsFalse(result);
//		chestData.info.Add(repGoodsData);
//		result = treasureChestCommandGroup.HandleChests(chestData,null);
//		Assert.IsTrue(result);
//		chestData.info.Add(repGoodsData1);
//		result = treasureChestCommandGroup.HandleChests(chestData,null);
//		Assert.IsTrue(result);
//		Assert.GreaterOrEqual(chestData.info[0].priority, chestData.info[1].priority);
//	}

//	[Test]
//	public void HandleEventFunction()
//	{
//		TreasureChestCommandGroup treasureChestCommandGroup = new TreasureChestCommandGroup(null, null, null, null);
//		int idCount = (int)treasureChestCommandGroup.HandleEventFunction(null);
//		Assert.AreEqual(idCount, 0);
//		idCount = (int)treasureChestCommandGroup.HandleEventFunction("123");
//		Assert.AreEqual(idCount, 1);
//		idCount = (int)treasureChestCommandGroup.HandleEventFunction("111", "123");
//		Assert.AreEqual(idCount, 2);
//	}

//	[Test]
//	public void GetSprite()
//	{
//		RewardTreasureChestDialog chestDialog = new RewardTreasureChestDialog();
//		Assert.NotNull(chestDialog);
//	}
//	// A UnityTest behaves like a coroutine in PlayMode
//	// and allows you to yield null to skip a frame in EditMode
//	[UnityTest]
//	public IEnumerator TreasureChestTestWithEnumeratorPasses() {
//		// Use the Assert class to test conditions.
//		// yield to skip a frame
//		yield return null;
//	}
//}
