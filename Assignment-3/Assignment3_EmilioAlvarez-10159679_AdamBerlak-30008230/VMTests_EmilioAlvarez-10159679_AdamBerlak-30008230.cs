/* Authors: Emilio Alejandro Alvarez Veronesi, 	Adam Berlak
UCIDs: 		10159679, 							30008230
Lab Section: B05
University e-mail: emilio.alvarezverone@ucalgary.ca, adam.berlak@ucalgary.ca
----------------------------------------------------

Classes:
	- GoodTests: All the good test scripts translated into unit tests
	- BadTests: Bad test scripts translated to unit tests

----------------------------------------------------

Versions:
	0.2 [2017-03-06]:
		- oops no versions in this one
		
	0.1 [2017-02-16]:
		- N/A
	
Bugs:
	-
To-Do List:
	- 

Notes:
	-

----------------------------------------------------
Acknowledgements and Sources:
	- Ochreus, rebertbu, "C# Adding Multiple Elements to a List on One Line", unity3d, 26 Aug. 2013
		link: http://answers.unity3d.com/questions/524128/c-adding-multiple-elements-to-a-list-on-one-line.html
*/
using System;
using System.Collections.Generic;
using Frontend2;
using Frontend2.Hardware;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class GoodTests
    {
        [TestMethod]
        public void T01_good_insert_and_press_exact_change(){
			int[] coinKinds = new int[]{5, 10, 25, 100};						//Make the appropriate list of coin kinds
			
			VendingMachine VM1 = new VendingMachine(coinKinds, 3, 10, 10, 10);	//Create the vending machine
			new VendingMachineLogic(VM1);										//Create a corresponding logic object
			
			//Configure the vending machine
			List<String> popNames = new List<String>(new String[]{"Coke", "water", "stuff"});
			List<int> popPrices = new List<int>(new int[]{250, 250, 205});
			VM1.Configure(popNames, popPrices);
			
			//Load coins
			VM1.CoinRacks[0].LoadCoins(new List<Coin>(new Coin[]{new Coin(5)}));
			VM1.CoinRacks[1].LoadCoins(new List<Coin>(new Coin[]{new Coin(10)}));
			VM1.CoinRacks[2].LoadCoins(new List<Coin>(new Coin[]{new Coin(25), new Coin(25)}));
			VM1.CoinRacks[3].LoadCoins(new List<Coin>(new Coin[]{}));
			
			//Load pops
			VM1.PopCanRacks[0].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("Coke")}));
			VM1.PopCanRacks[1].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("water")}));
			VM1.PopCanRacks[2].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("stuff")}));
			
			//Insert coins
			VM1.CoinSlot.AddCoin(new Coin(100));
			VM1.CoinSlot.AddCoin(new Coin(100));
			VM1.CoinSlot.AddCoin(new Coin(25));
			VM1.CoinSlot.AddCoin(new Coin(25));
			
			//Press button
			VM1.SelectionButtons[0].Press();
			
			//Extract from delivery chute and check delivery
			IDeliverable[] delivered = VM1.DeliveryChute.RemoveItems();
			int expectedChangeInDeliveryChute = 0;
			Dictionary<string, int> expectedPopInDeliveryChute = new Dictionary<string, int>();
			expectedPopInDeliveryChute["Coke"] = 1;
			Assert.IsTrue(checkDelivery(delivered, expectedChangeInDeliveryChute, expectedPopInDeliveryChute));
			
			//Unload VM and check teardown
			int expectedChangeInCoinRacks = 315;
			int expectedChangeInStorageBin = 0;
			Dictionary<string, int> expectedPopInPopRacks = new Dictionary<string, int>();
			expectedPopInPopRacks["water"] = 1;
			expectedPopInPopRacks["stuff"] = 1;
			Assert.IsTrue(unloadAndCheck(VM1, expectedChangeInCoinRacks, expectedChangeInStorageBin, expectedPopInPopRacks));
		}

        [TestMethod]
        public void T02_good_insert_and_press_change_expected(){
			int[] coinKinds = new int[]{5, 10, 25, 100};						//Make the appropriate list of coin kinds
			
			VendingMachine VM1 = new VendingMachine(coinKinds, 3, 10, 10, 10);	//Create the vending machine
			new VendingMachineLogic(VM1);										//Create a corresponding logic object
			
			//Configure the vending machine
			List<String> popNames = new List<String>(new String[]{"Coke", "water", "stuff"});
			List<int> popPrices = new List<int>(new int[]{250, 250, 205});
			VM1.Configure(popNames, popPrices);
			
			//Load coins
			VM1.CoinRacks[0].LoadCoins(new List<Coin>(new Coin[]{new Coin(5)}));
			VM1.CoinRacks[1].LoadCoins(new List<Coin>(new Coin[]{new Coin(10)}));
			VM1.CoinRacks[2].LoadCoins(new List<Coin>(new Coin[]{new Coin(25), new Coin(25)}));
			VM1.CoinRacks[3].LoadCoins(new List<Coin>(new Coin[]{}));
			
			//Load pops
			VM1.PopCanRacks[0].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("Coke")}));
			VM1.PopCanRacks[1].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("water")}));
			VM1.PopCanRacks[2].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("stuff")}));
			
			//Insert coins
			VM1.CoinSlot.AddCoin(new Coin(100));
			VM1.CoinSlot.AddCoin(new Coin(100));
			VM1.CoinSlot.AddCoin(new Coin(100));
			
			//Press button
			VM1.SelectionButtons[0].Press();
			
			//Extract from delivery chute and check delivery
			IDeliverable[] delivered = VM1.DeliveryChute.RemoveItems();
			int expectedChangeInDeliveryChute = 50;
			Dictionary<string, int> expectedPopInDeliveryChute = new Dictionary<string, int>();
			expectedPopInDeliveryChute["Coke"] = 1;
			Assert.IsTrue(checkDelivery(delivered, expectedChangeInDeliveryChute, expectedPopInDeliveryChute));
			
			//Unload VM and check teardown
			int expectedChangeInCoinRacks = 315;
			int expectedChangeInStorageBin = 0;
			Dictionary<string, int> expectedPopInPopRacks = new Dictionary<string, int>();
			expectedPopInPopRacks["water"] = 1;
			expectedPopInPopRacks["stuff"] = 1;
			Assert.IsTrue(unloadAndCheck(VM1, expectedChangeInCoinRacks, expectedChangeInStorageBin, expectedPopInPopRacks));
		}

        [TestMethod]
        public void T03_good_teardown_without_configure_or_load(){
			int[] coinKinds = new int[]{5, 10, 25, 100};						//Make the appropriate list of coin kinds
			
			VendingMachine VM1 = new VendingMachine(coinKinds, 3, 10, 10, 10);	//Create the vending machine
			new VendingMachineLogic(VM1);										//Create a corresponding logic object
			
			//Check the delivery
			IDeliverable[] delivered = VM1.DeliveryChute.RemoveItems();			//Remove items from the delivery chute
			int expectedChangeInDeliveryChute = 0;
			Assert.IsTrue(checkDelivery(delivered, expectedChangeInDeliveryChute));
			
			//Unload the machine, and check the result
			int expectedChangeInCoinRacks = 0;
			int expectedChangeInStorageBin = 0;
			Assert.IsTrue(unloadAndCheck(VM1, expectedChangeInCoinRacks, expectedChangeInStorageBin));	
        }

        [TestMethod]
        public void T04_good_press_without_insert(){
			int[] coinKinds = new int[]{5, 10, 25, 100};						//Make the appropriate list of coin kinds
			
			VendingMachine VM1 = new VendingMachine(coinKinds, 3, 10, 10, 10);	//Create the vending machine
			new VendingMachineLogic(VM1);										//Create a corresponding logic object
			
			//Configure the vending machine
			List<String> popNames = new List<String>(new String[]{"Coke", "water", "stuff"});
			List<int> popPrices = new List<int>(new int[]{250, 250, 205});
			VM1.Configure(popNames, popPrices);
			
			//Load coins
			VM1.CoinRacks[0].LoadCoins(new List<Coin>(new Coin[]{new Coin(5)}));
			VM1.CoinRacks[1].LoadCoins(new List<Coin>(new Coin[]{new Coin(10)}));
			VM1.CoinRacks[2].LoadCoins(new List<Coin>(new Coin[]{new Coin(25), new Coin(25)}));
			VM1.CoinRacks[3].LoadCoins(new List<Coin>(new Coin[]{}));
			
			//Load pops
			VM1.PopCanRacks[0].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("Coke")}));
			VM1.PopCanRacks[1].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("water")}));
			VM1.PopCanRacks[2].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("stuff")}));
			
			//Press button
			VM1.SelectionButtons[0].Press();
			
			//Extract from delivery chute and check delivery
			IDeliverable[] delivered = VM1.DeliveryChute.RemoveItems();
			int expectedChangeInDeliveryChute = 0;
			Assert.IsTrue(checkDelivery(delivered, expectedChangeInDeliveryChute));
			
			//Unload VM and check teardown
			int expectedChangeInCoinRacks = 65;
			int expectedChangeInStorageBin = 0;
			Dictionary<string, int> expectedPopInPopRacks = new Dictionary<string, int>();
			expectedPopInPopRacks["Coke"] = 1;
			expectedPopInPopRacks["water"] = 1;
			expectedPopInPopRacks["stuff"] = 1;
			Assert.IsTrue(unloadAndCheck(VM1, expectedChangeInCoinRacks, expectedChangeInStorageBin, expectedPopInPopRacks));
		}

        [TestMethod]
        public void T05_good_scrambled_coin_kinds(){
			int[] coinKinds = new int[]{100, 5, 25, 10};						//Make the appropriate list of coin kinds
			
			VendingMachine VM1 = new VendingMachine(coinKinds, 3, 2, 10, 10);	//Create the vending machine
			new VendingMachineLogic(VM1);										//Create a corresponding logic object
			
			//Configure the vending machine
			List<String> popNames = new List<String>(new String[]{"Coke", "water", "stuff"});
			List<int> popPrices = new List<int>(new int[]{250, 250, 205});
			VM1.Configure(popNames, popPrices);
			
			//Load coins
			VM1.CoinRacks[0].LoadCoins(new List<Coin>(new Coin[]{}));
			VM1.CoinRacks[1].LoadCoins(new List<Coin>(new Coin[]{new Coin(5)}));
			VM1.CoinRacks[2].LoadCoins(new List<Coin>(new Coin[]{new Coin(25), new Coin(25)}));
			VM1.CoinRacks[3].LoadCoins(new List<Coin>(new Coin[]{new Coin(10)}));
			
			//Load pops
			VM1.PopCanRacks[0].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("Coke")}));
			VM1.PopCanRacks[1].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("water")}));
			VM1.PopCanRacks[2].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("stuff")}));
			
			//Press button
			VM1.SelectionButtons[0].Press();
			
			//Extract from delivery chute and check delivery
			IDeliverable[] delivered = VM1.DeliveryChute.RemoveItems();
			int expectedChangeInDeliveryChute = 0;
			Assert.IsTrue(checkDelivery(delivered, expectedChangeInDeliveryChute));
			
			//Insert coins
			VM1.CoinSlot.AddCoin(new Coin(100));
			VM1.CoinSlot.AddCoin(new Coin(100));
			VM1.CoinSlot.AddCoin(new Coin(100));
			
			//Press button
			VM1.SelectionButtons[0].Press();
			
			//Extract from delivery chute and check delivery
			delivered = VM1.DeliveryChute.RemoveItems();
			expectedChangeInDeliveryChute = 50;
			Dictionary<string, int> expectedPopInDeliveryChute = new Dictionary<string, int>();
			expectedPopInDeliveryChute["Coke"] = 1;
			Assert.IsTrue(checkDelivery(delivered, expectedChangeInDeliveryChute, expectedPopInDeliveryChute));
			
			//Unload VM and check teardown
			int expectedChangeInCoinRacks = 215;
			int expectedChangeInStorageBin = 100;
			Dictionary<string, int> expectedPopInPopRacks = new Dictionary<string, int>();
			expectedPopInPopRacks["water"] = 1;
			expectedPopInPopRacks["stuff"] = 1;
			Assert.IsTrue(unloadAndCheck(VM1, expectedChangeInCoinRacks, expectedChangeInStorageBin, expectedPopInPopRacks));
		}

        [TestMethod]
        public void T06_good_extract_before_sale(){
			int[] coinKinds = new int[]{100, 5, 25, 10};						//Make the appropriate list of coin kinds
			
			VendingMachine VM1 = new VendingMachine(coinKinds, 3, 10, 10, 10);	//Create the vending machine
			new VendingMachineLogic(VM1);										//Create a corresponding logic object
			
			//Configure the vending machine
			List<String> popNames = new List<String>(new String[]{"Coke", "water", "stuff"});
			List<int> popPrices = new List<int>(new int[]{250, 250, 205});
			VM1.Configure(popNames, popPrices);
			
			//Load coins
			VM1.CoinRacks[0].LoadCoins(new List<Coin>(new Coin[]{}));
			VM1.CoinRacks[1].LoadCoins(new List<Coin>(new Coin[]{new Coin(5)}));
			VM1.CoinRacks[2].LoadCoins(new List<Coin>(new Coin[]{new Coin(25), new Coin(25)}));
			VM1.CoinRacks[3].LoadCoins(new List<Coin>(new Coin[]{new Coin(10)}));
			
			//Load pops
			VM1.PopCanRacks[0].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("Coke")}));
			VM1.PopCanRacks[1].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("water")}));
			VM1.PopCanRacks[2].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("stuff")}));
			
			//Press button
			VM1.SelectionButtons[0].Press();
			
			//Extract from delivery chute and check delivery
			IDeliverable[] delivered = VM1.DeliveryChute.RemoveItems();
			int expectedChangeInDeliveryChute = 0;
			Assert.IsTrue(checkDelivery(delivered, expectedChangeInDeliveryChute));
			
			//Insert coins
			VM1.CoinSlot.AddCoin(new Coin(100));
			VM1.CoinSlot.AddCoin(new Coin(100));
			VM1.CoinSlot.AddCoin(new Coin(100));
			
			//Extract from delivery chute and check delivery
			delivered = VM1.DeliveryChute.RemoveItems();
			expectedChangeInDeliveryChute = 0;
			Assert.IsTrue(checkDelivery(delivered, expectedChangeInDeliveryChute));
			
			//Unload VM and check teardown
			int expectedChangeInCoinRacks = 65;
			int expectedChangeInStorageBin = 0;
			Dictionary<string, int> expectedPopInPopRacks = new Dictionary<string, int>();
			expectedPopInPopRacks["Coke"] = 1;
			expectedPopInPopRacks["water"] = 1;
			expectedPopInPopRacks["stuff"] = 1;
			Assert.IsTrue(unloadAndCheck(VM1, expectedChangeInCoinRacks, expectedChangeInStorageBin, expectedPopInPopRacks));
		}

        [TestMethod]
        public void T07_good_changing_configuration(){
			int[] coinKinds = new int[]{5, 10, 25, 100};						//Make the appropriate list of coin kinds
			
			VendingMachine VM1 = new VendingMachine(coinKinds, 3, 10, 10, 10);	//Create the vending machine
			new VendingMachineLogic(VM1);										//Create a corresponding logic object
			
			//Configure the vending machine
			List<String> popNames = new List<String>(new String[]{"A", "B", "C"});
			List<int> popPrices = new List<int>(new int[]{5, 10, 25});
			VM1.Configure(popNames, popPrices);
			
			//Load coins
			VM1.CoinRacks[0].LoadCoins(new List<Coin>(new Coin[]{new Coin(5)}));
			VM1.CoinRacks[1].LoadCoins(new List<Coin>(new Coin[]{new Coin(10)}));
			VM1.CoinRacks[2].LoadCoins(new List<Coin>(new Coin[]{new Coin(25), new Coin(25)}));
			VM1.CoinRacks[3].LoadCoins(new List<Coin>(new Coin[]{}));
			
			//Load pops
			VM1.PopCanRacks[0].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("A")}));
			VM1.PopCanRacks[1].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("B")}));
			VM1.PopCanRacks[2].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("C")}));
			
			//Re-configure the vending machine
			popNames = new List<String>(new String[]{"Coke", "water", "stuff"});
			popPrices = new List<int>(new int[]{250, 250, 205});
			VM1.Configure(popNames, popPrices);
			
			//Press button
			VM1.SelectionButtons[0].Press();
			
			//Extract from delivery chute and check delivery
			IDeliverable[] delivered = VM1.DeliveryChute.RemoveItems();
			int expectedChangeInDeliveryChute = 0;
			Assert.IsTrue(checkDelivery(delivered, expectedChangeInDeliveryChute));
			
			//Insert coins
			VM1.CoinSlot.AddCoin(new Coin(100));
			VM1.CoinSlot.AddCoin(new Coin(100));
			VM1.CoinSlot.AddCoin(new Coin(100));
			
			//Press button
			VM1.SelectionButtons[0].Press();
			
			//Extract from delivery chute and check delivery
			delivered = VM1.DeliveryChute.RemoveItems();
			expectedChangeInDeliveryChute = 50;
			Dictionary<string, int> expectedPopInDeliveryChute = new Dictionary<string, int>();
			expectedPopInDeliveryChute["A"] = 1;
			Assert.IsTrue(checkDelivery(delivered, expectedChangeInDeliveryChute, expectedPopInDeliveryChute));
			
			//Unload VM and check teardown
			int expectedChangeInCoinRacks = 315;
			int expectedChangeInStorageBin = 0;
			Dictionary<string, int> expectedPopInPopRacks = new Dictionary<string, int>();
			expectedPopInPopRacks["B"] = 1;
			expectedPopInPopRacks["C"] = 1;
			Assert.IsTrue(unloadAndCheck(VM1, expectedChangeInCoinRacks, expectedChangeInStorageBin, expectedPopInPopRacks));
			
			//Load coins again
			VM1.CoinRacks[0].LoadCoins(new List<Coin>(new Coin[]{new Coin(5)}));
			VM1.CoinRacks[1].LoadCoins(new List<Coin>(new Coin[]{new Coin(10)}));
			VM1.CoinRacks[2].LoadCoins(new List<Coin>(new Coin[]{new Coin(25), new Coin(25)}));
			VM1.CoinRacks[3].LoadCoins(new List<Coin>(new Coin[]{}));
			
			//Load pops again
			VM1.PopCanRacks[0].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("Coke")}));
			VM1.PopCanRacks[1].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("water")}));
			VM1.PopCanRacks[2].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("stuff")}));
			
			//Insert coins
			VM1.CoinSlot.AddCoin(new Coin(100));
			VM1.CoinSlot.AddCoin(new Coin(100));
			VM1.CoinSlot.AddCoin(new Coin(100));
			
			//Press button
			VM1.SelectionButtons[0].Press();
			
			//Extract from delivery chute and check delivery
			delivered = VM1.DeliveryChute.RemoveItems();
			expectedChangeInDeliveryChute = 50;
			expectedPopInDeliveryChute = new Dictionary<string, int>();
			expectedPopInDeliveryChute["Coke"] = 1;
			Assert.IsTrue(checkDelivery(delivered, expectedChangeInDeliveryChute, expectedPopInDeliveryChute));
			
			//Unload VM and check teardown
			expectedChangeInCoinRacks = 315;
			expectedChangeInStorageBin = 0;
			expectedPopInPopRacks = new Dictionary<string, int>();
			expectedPopInPopRacks["water"] = 1;
			expectedPopInPopRacks["stuff"] = 1;
			Assert.IsTrue(unloadAndCheck(VM1, expectedChangeInCoinRacks, expectedChangeInStorageBin, expectedPopInPopRacks));
		}

        [TestMethod]
        public void T08_good_approximate_change(){
			//Create the vending machine
			int[] coinKinds = new int[4] { 5, 10, 25, 100 };
            int selectionButtonCount = 1; int coinRackCapacity = 10; int popCanRackCapacity = 10; int receptacleCapacity = 10;
            VendingMachine vm = new VendingMachine(coinKinds, selectionButtonCount, coinRackCapacity, popCanRackCapacity, receptacleCapacity);

			//Configure the vending machine
            List<String> popCanNames = new List<string>(new string[] { "stuff" });
            List<int> popCanCosts = new List<int>(new int[] { 140 });
            vm.Configure(popCanNames, popCanCosts);

			//Load coins
            int[] coinCounts = new int[4] { 0, 5, 1, 1 };
            vm.LoadCoins(coinCounts);

			//Load pops
            int[] popCanCounts = new int[1] { 1 };
            vm.LoadPopCans(popCanCounts);

			//Assign a logic class to the machine
            VendingMachineLogic vml = new VendingMachineLogic(vm);

			//Insert coins
            vm.CoinSlot.AddCoin(new Coin(100));
            vm.CoinSlot.AddCoin(new Coin(100));
            vm.CoinSlot.AddCoin(new Coin(100));

			//Press button
            vm.SelectionButtons[0].Press();

			//Check delivery
            IDeliverable[] result = vm.DeliveryChute.RemoveItems();
            Assert.AreEqual(155, this.DeliverySum(result));
            CollectionAssert.AreEqual(new PopCan[] { new PopCan("stuff") }, this.DeliveryPops(result));

			//Check teardown
            VendingMachineStoredContents result2 = this.UnloadVendingMachine(vm);
            Assert.AreEqual(320, this.CoinRackSum(result2.CoinsInCoinRacks));
            Assert.AreEqual(0, this.StorageSum(result2.PaymentCoinsInStorageBin));
            CollectionAssert.AreEqual(new PopCan[0], this.PopRackList(result2.PopCansInPopCanRacks));
		}

        [TestMethod]
        public void T09_good_hard_for_change(){
			//Create the vending machine
			int[] coinKinds = new int[4] { 5, 10, 25, 100 };
            int selectionButtonCount = 1; int coinRackCapacity = 10; int popCanRackCapacity = 10; int receptacleCapacity = 10;
            VendingMachine vm = new VendingMachine(coinKinds, selectionButtonCount, coinRackCapacity, popCanRackCapacity, receptacleCapacity);

			//Configure the vending machine
            List<String> popCanNames = new List<string>(new string[] { "stuff" });
            List<int> popCanCosts = new List<int>(new int[] { 140 });
            vm.Configure(popCanNames, popCanCosts);

			//Load coins
            int[] coinCounts = new int[4] { 1, 6, 1, 1 };
            vm.LoadCoins(coinCounts);

			//Load pops
            int[] popCanCounts = new int[1] { 1 };
            vm.LoadPopCans(popCanCounts);

			//Assign a logic class to the vending machine
            VendingMachineLogic vml = new VendingMachineLogic(vm);

			//Insert coins
            vm.CoinSlot.AddCoin(new Coin(100));
            vm.CoinSlot.AddCoin(new Coin(100));
            vm.CoinSlot.AddCoin(new Coin(100));

			//Press button
            vm.SelectionButtons[0].Press();

			//Check the delivery
            IDeliverable[] result = vm.DeliveryChute.RemoveItems();
            Assert.AreEqual(160, this.DeliverySum(result));
            CollectionAssert.AreEqual(new PopCan[] { new PopCan("stuff") }, this.DeliveryPops(result));

			//Check teardown
            VendingMachineStoredContents result2 = this.UnloadVendingMachine(vm);
            Assert.AreEqual(330, this.CoinRackSum(result2.CoinsInCoinRacks));
            Assert.AreEqual(0, this.StorageSum(result2.PaymentCoinsInStorageBin));
            CollectionAssert.AreEqual(new PopCan[0], this.PopRackList(result2.PopCansInPopCanRacks));
		}

        [TestMethod]
        public void T10_good_invalid_coin(){
			//Create the vending machine
			int[] coinKinds = new int[4] { 5, 10, 25, 100 };
            int selectionButtonCount = 1; int coinRackCapacity = 10; int popCanRackCapacity = 10; int receptacleCapacity = 10;
            VendingMachine vm = new VendingMachine(coinKinds, selectionButtonCount, coinRackCapacity, popCanRackCapacity, receptacleCapacity);

			//Configure the vending machine
            List<String> popCanNames = new List<string>(new string[] { "stuff" });
            List<int> popCanCosts = new List<int>(new int[] { 140 });
            vm.Configure(popCanNames, popCanCosts);

			//Load coins
            int[] coinCounts = new int[4] { 1, 6, 1, 1 };
            vm.LoadCoins(coinCounts);

			//Load pops
            int[] popCanCounts = new int[1] { 1 };
            vm.LoadPopCans(popCanCounts);

			//Assign a logic class to the vending machine
            VendingMachineLogic vml = new VendingMachineLogic(vm);

			//Insert coins
            vm.CoinSlot.AddCoin(new Coin(1));
            vm.CoinSlot.AddCoin(new Coin(139));

			//Press button
            vm.SelectionButtons[0].Press();

			//Check delivery
            IDeliverable[] result = vm.DeliveryChute.RemoveItems();
            Assert.AreEqual(140, this.DeliverySum(result));
            CollectionAssert.AreEqual(new PopCan[0], this.DeliveryPops(result));

			//Check teardown
            VendingMachineStoredContents result2 = this.UnloadVendingMachine(vm);
            Assert.AreEqual(190, this.CoinRackSum(result2.CoinsInCoinRacks));
            Assert.AreEqual(0, this.StorageSum(result2.PaymentCoinsInStorageBin));
            CollectionAssert.AreEqual(new PopCan[] { new PopCan("stuff") }, this.PopRackList(result2.PopCansInPopCanRacks));
		}

        [TestMethod]
        public void T11_good_extract_before_sale_complex(){
			//Create the vending machine
			int[] coinKinds = new int[4] { 100, 5, 25, 10 };
            int selectionButtonCount = 3; int coinRackCapacity = 10; int popCanRackCapacity = 10; int receptacleCapacity = 10;
            VendingMachine vm = new VendingMachine(coinKinds, selectionButtonCount, coinRackCapacity, popCanRackCapacity, receptacleCapacity);

			//Configure the vending machine
            List<String> popCanNames = new List<string>(new string[] { "Coke", "water", "stuff" });
            List<int> popCanCosts = new List<int>(new int[] { 250, 250, 205 });
            vm.Configure(popCanNames, popCanCosts);

			//Load coins
            int[] coinCounts = new int[4] { 0, 1, 2, 1 };
            vm.LoadCoins(coinCounts);

			//Load pops
            int[] popCanCounts = new int[3] { 1, 1, 1 };
            vm.LoadPopCans(popCanCounts);

			//Assign a logic class to the vending machine
            VendingMachineLogic vml = new VendingMachineLogic(vm);

			//Press button
            vm.SelectionButtons[0].Press();

			//Check delivery
            IDeliverable[] result = vm.DeliveryChute.RemoveItems();
            Assert.AreEqual(0, this.DeliverySum(result));
            CollectionAssert.AreEqual(new PopCan[0], this.DeliveryPops(result));

			//Insert coins
            vm.CoinSlot.AddCoin(new Coin(100));
            vm.CoinSlot.AddCoin(new Coin(100));
            vm.CoinSlot.AddCoin(new Coin(100));

			//Check delivery
            IDeliverable[] result2 = vm.DeliveryChute.RemoveItems();
            Assert.AreEqual(0, this.DeliverySum(result));
            CollectionAssert.AreEqual(new PopCan[0], this.DeliveryPops(result2));

			//Check teardown
            VendingMachineStoredContents result3 = this.UnloadVendingMachine(vm);
            System.Console.WriteLine(this.CoinRackSum(result3.CoinsInCoinRacks));
            Assert.AreEqual(65, this.CoinRackSum(result3.CoinsInCoinRacks));
            Assert.AreEqual(0, this.StorageSum(result3.PaymentCoinsInStorageBin));
            CollectionAssert.AreEqual(new PopCan[] { new PopCan("Coke"), new PopCan("water"), new PopCan("stuff") }, this.PopRackList(result3.PopCansInPopCanRacks));

			//Load coins
            coinCounts = new int[4] { 0, 1, 2, 1 };
            vm.LoadCoins(coinCounts);

			//Load pops
            popCanCounts = new int[3] { 1, 2, 1 };
            vm.LoadPopCans(popCanCounts);

			//Press button
            vm.SelectionButtons[0].Press();

			//Check delivery
            IDeliverable[] result4 = vm.DeliveryChute.RemoveItems();
            Assert.AreEqual(50, this.DeliverySum(result4));
            CollectionAssert.AreEqual(new PopCan[] { new PopCan("Coke") }, this.DeliveryPops(result4));

			//Check teardown
            VendingMachineStoredContents result5 = this.UnloadVendingMachine(vm);
            Assert.AreEqual(315, this.CoinRackSum(result5.CoinsInCoinRacks));
            Assert.AreEqual(0, this.StorageSum(result5.PaymentCoinsInStorageBin));
            CollectionAssert.AreEqual(new PopCan[] {new PopCan("water"), new PopCan("stuff") }, this.PopRackList(result5.PopCansInPopCanRacks));

			//Create new vending machine
            coinKinds = new int[4] { 100, 5, 25, 10 };
            selectionButtonCount = 3; coinRackCapacity = 10; popCanRackCapacity = 10; receptacleCapacity = 10;
            VendingMachine vm2 = new VendingMachine(coinKinds, selectionButtonCount, coinRackCapacity, popCanRackCapacity, receptacleCapacity);

			//Configure the new vending machine twice
            popCanNames = new List<string>(new string[] { "Coke", "water", "stuff" });
            popCanCosts = new List<int>(new int[] { 250, 250, 205 });
            vm2.Configure(popCanNames, popCanCosts);
            popCanNames = new List<string>(new string[] { "A", "B", "C" });
            popCanCosts = new List<int>(new int[] { 5, 10, 25 });
            vm2.Configure(popCanNames, popCanCosts);

			//Assign a vending machine logic class to the new vending machine
            VendingMachineLogic vml2 = new VendingMachineLogic(vm2);

			//Check teardown
			VendingMachineStoredContents result6 = this.UnloadVendingMachine(vm2);
            Assert.AreEqual(0, this.CoinRackSum(result6.CoinsInCoinRacks));
            Assert.AreEqual(0, this.StorageSum(result6.PaymentCoinsInStorageBin));
            CollectionAssert.AreEqual(new PopCan[] {}, this.PopRackList(result6.PopCansInPopCanRacks));

			//Load coins
            coinCounts = new int[4] { 0, 1, 2, 1 };
            vm2.LoadCoins(coinCounts);

			//Load pops
            popCanCounts = new int[3] { 1, 1, 1 };
            vm2.LoadPopCans(popCanCounts);

			//Insert coins
            vm2.CoinSlot.AddCoin(new Coin(10));
            vm2.CoinSlot.AddCoin(new Coin(5));
            vm2.CoinSlot.AddCoin(new Coin(10));

			//Press button
            vm2.SelectionButtons[2].Press();

			//Check delivery
            IDeliverable[] result7 = vm2.DeliveryChute.RemoveItems();
            Assert.AreEqual(0, this.DeliverySum(result7));
            System.Console.WriteLine(result7.Length);
            CollectionAssert.AreEqual(new PopCan[] { new PopCan("C") }, this.DeliveryPops(result7));

			//Check teardown
            VendingMachineStoredContents result8 = this.UnloadVendingMachine(vm2);
            Assert.AreEqual(90, this.CoinRackSum(result8.CoinsInCoinRacks));
            Assert.AreEqual(0, this.StorageSum(result8.PaymentCoinsInStorageBin));
            CollectionAssert.AreEqual(new PopCan[] { new PopCan("A"), new PopCan("B") }, this.PopRackList(result8.PopCansInPopCanRacks));
		}

        [TestMethod]
        public void T12_good_approximate_change_with_credit(){
			//Create the vending machine
			int[] coinKinds = new int[4] { 5, 10, 25, 100 };
            int selectionButtonCount = 1; int coinRackCapacity = 10; int popCanRackCapacity = 10; int receptacleCapacity = 10;
            VendingMachine vm = new VendingMachine(coinKinds, selectionButtonCount, coinRackCapacity, popCanRackCapacity, receptacleCapacity);

			//Configure the vending machine
            List<String> popCanNames = new List<string>(new string[] { "stuff" });
            List<int> popCanCosts = new List<int>(new int[] { 140 });
            vm.Configure(popCanNames, popCanCosts);

			//Load coins
            int[] coinCounts = new int[4] { 0, 5, 1, 1 };
            vm.LoadCoins(coinCounts);

			//Load pops
            int[] popCanCounts = new int[1] { 1 };
            vm.LoadPopCans(popCanCounts);

			//Assign a logic class to the vm
            VendingMachineLogic vml = new VendingMachineLogic(vm);

			//Insert coins
            vm.CoinSlot.AddCoin(new Coin(100));
            vm.CoinSlot.AddCoin(new Coin(100));
            vm.CoinSlot.AddCoin(new Coin(100));

			//Press button
            vm.SelectionButtons[0].Press();

			//Check delivery
            IDeliverable[] result = vm.DeliveryChute.RemoveItems();
            Assert.AreEqual(155, this.DeliverySum(result));
            CollectionAssert.AreEqual(new PopCan[] { new PopCan("stuff") }, this.DeliveryPops(result));

			//Check teardown
            VendingMachineStoredContents result2 = this.UnloadVendingMachine(vm);
            Assert.AreEqual(320, this.CoinRackSum(result2.CoinsInCoinRacks));
            Assert.AreEqual(0, this.StorageSum(result2.PaymentCoinsInStorageBin));
            CollectionAssert.AreEqual(new PopCan[0], this.PopRackList(result2.PopCansInPopCanRacks));

			//Load coins again
            coinCounts = new int[4] { 10, 10, 10, 10 };
            vm.LoadCoins(coinCounts);

			//Load pops again
            popCanCounts = new int[1] { 1 };
            vm.LoadPopCans(popCanCounts);

			//Insert coins
            vm.CoinSlot.AddCoin(new Coin(25));
            vm.CoinSlot.AddCoin(new Coin(100));
            vm.CoinSlot.AddCoin(new Coin(10));

			//Press button
            vm.SelectionButtons[0].Press();

			//Check delivery
            IDeliverable[] result3 = vm.DeliveryChute.RemoveItems();
            Assert.AreEqual(0, this.DeliverySum(result3));
            CollectionAssert.AreEqual(new PopCan[] { new PopCan("stuff") }, this.DeliveryPops(result3));

			//Check teardown
            VendingMachineStoredContents result4 = this.UnloadVendingMachine(vm);
            Assert.AreEqual(1400, this.CoinRackSum(result4.CoinsInCoinRacks));
            Assert.AreEqual(135, this.StorageSum(result4.PaymentCoinsInStorageBin));
            CollectionAssert.AreEqual(new PopCan[0], this.PopRackList(result4.PopCansInPopCanRacks));
		}

        [TestMethod]
        public void T13_good_need_to_store_payment(){
			//Create the vending machine
			int[] coinKinds = new int[4] { 5, 10, 25, 100 };
            int selectionButtonCount = 1; int coinRackCapacity = 10; int popCanRackCapacity = 10; int receptacleCapacity = 10;
            VendingMachine vm = new VendingMachine(coinKinds, selectionButtonCount, coinRackCapacity, popCanRackCapacity, receptacleCapacity);

			//Configure the vending machine
            List<String> popCanNames = new List<string>(new string[] { "stuff" });
            List<int> popCanCosts = new List<int>(new int[] { 135 });
            vm.Configure(popCanNames, popCanCosts);

			//Load coins
            int[] coinCounts = new int[4] { 10, 10, 10, 10 };
            vm.LoadCoins(coinCounts);

			//Load pops
            int[] popCanCounts = new int[1] { 1 };
            vm.LoadPopCans(popCanCounts);

			//Assign a logic class the the vm
            VendingMachineLogic vml = new VendingMachineLogic(vm);

			//Insert coins
            vm.CoinSlot.AddCoin(new Coin(25));
            vm.CoinSlot.AddCoin(new Coin(100));
            vm.CoinSlot.AddCoin(new Coin(10));

			//Press button
            vm.SelectionButtons[0].Press();

			//Check delivery
            IDeliverable[] result = vm.DeliveryChute.RemoveItems();
            Assert.AreEqual(0, this.DeliverySum(result));
            CollectionAssert.AreEqual(new PopCan[] { new PopCan("stuff") }, this.DeliveryPops(result));

			//Check teardown
            VendingMachineStoredContents result2 = this.UnloadVendingMachine(vm);
            Assert.AreEqual(1400, this.CoinRackSum(result2.CoinsInCoinRacks));
            Assert.AreEqual(135, this.StorageSum(result2.PaymentCoinsInStorageBin));
            CollectionAssert.AreEqual(new PopCan[0], this.PopRackList(result2.PopCansInPopCanRacks));
		}
		
		//Methods to check the deliveries and teardowns.
		private bool checkDelivery(IDeliverable[] deliveredItems, int expectedChange, Dictionary<string, int> expectedPop){			
			foreach (var item in deliveredItems)						//Get each item in the delivery chute
			{
				if (item is PopCan)										//If the item is a pop
				{
					if (expectedPop.ContainsKey(((PopCan) item).Name))	//Check that the pop type is expected
						expectedPop[((PopCan) item).Name] -= 1;			//If it is expected, reduce the expected amount
					else
						return false;									//If the pop type is not expected, return false
				}
				else if (item is Coin)									//If the item is a coin,
					expectedChange -= ((Coin) item).Value;				//Reduce the expected change amount by the coin's value
			}
			
			foreach (int amount in expectedPop.Values)	//Make sure that the amount of each expected pop is zero
			{
				if (amount != 0)
					return false;						//If any expected pop is not 0, return false
			}
			if (expectedChange != 0)
				return false;							//If the amount of expected change is not 0, return false
			
			return true;	//If false is never returned, then the delivery check passes
		}
		private bool checkDelivery(IDeliverable[] deliveredItems, int expectedChange){
			foreach (var item in deliveredItems)			//Get each item in the delivery chute
			{
				if (item is Coin)							//If the item is a coin, reduce the expected change amount
					expectedChange -= ((Coin) item).Value;
				else
					return false;							//If there is something else (PopCan), return false
			}
			
			if (expectedChange != 0)	//If the expected change is not 0 at this point, return false
				return false;
			
			return true;				//Otherwise return true
		}
		
		private bool unloadAndCheck(VendingMachine vm, int expectedChange, int expectedGross, Dictionary<string, int> expectedPop){
			List<List<Coin>> coinRacks = new List<List<Coin>>();
			List<Coin> coinsInStorage;
			List<List<PopCan>> popRacks = new List<List<PopCan>>();
			
			foreach (var coinRack in vm.CoinRacks)
			{
				coinRacks.Add(coinRack.Unload());
			}
			coinsInStorage = vm.StorageBin.Unload();
			foreach (var popRack in vm.PopCanRacks)
			{
				popRacks.Add(popRack.Unload());
			}
			
			return (checkTeardown(coinRacks, coinsInStorage, popRacks, expectedChange, expectedGross, expectedPop));
		}
		private bool unloadAndCheck(VendingMachine vm, int expectedChange, int expectedGross){
			List<List<Coin>> coinRacks = new List<List<Coin>>();
			List<Coin> coinsInStorage;
			List<List<PopCan>> popRacks = new List<List<PopCan>>();
			
			foreach (var coinRack in vm.CoinRacks)
			{
				coinRacks.Add(coinRack.Unload());
			}
			coinsInStorage = vm.StorageBin.Unload();
			foreach (var popRack in vm.PopCanRacks)
			{
				popRacks.Add(popRack.Unload());
			}
			
			return (checkTeardown(coinRacks, coinsInStorage, popRacks, expectedChange, expectedGross));
		}
		
		private bool checkTeardown(List<List<Coin>> CoinsInCoinRacks, List<Coin> CoinsInStorage, List<List<PopCan>> PopInPopRacks, int expectedChange, int expectedGross, Dictionary<string, int> expectedPop){
			foreach (var coinRack in CoinsInCoinRacks)		//Get each coin rack
			{
				foreach (var coin in coinRack)				//Get each coin in the current coin rack
				{
					expectedChange -= coin.Value;			//Subtract the coin's value from the expected change
				}
			}
			foreach (var coin in CoinsInStorage)			//Get each coin in the storage bin
			{
				expectedGross -= coin.Value;				//Subtract the coin's value from the expected amount
			}
			foreach (var popRack in PopInPopRacks)			//Get each pop rack
			{
				foreach (var pop in popRack)				//Get each pop in the current pop rack
				{
					if (expectedPop.ContainsKey(pop.Name))	//If the expected pop dictionary contains the pop currently selected
						expectedPop[pop.Name] -= 1;			//Decrease the number of pops that are expected of the specific kind
					else
						return false;						//If the current pop is not exptected, return false
				}
			}
			
			foreach (int amount in expectedPop.Values)			//Check that all the expected amounts are zero
			{
				if (amount != 0)								//If an amount is not 0, return false
					return false;
			}
			if ((expectedChange != 0) || (expectedGross != 0))	//If either the expected change or expected gross is not zero, return false
				return false;
			
			return true;	//Otherwise return true, meaning the extracted items match the expected items
		}
		private bool checkTeardown(List<List<Coin>> CoinsInCoinRacks, List<Coin> CoinsInStorage, List<List<PopCan>> PopInPopRacks, int expectedChange, int expectedGross){
			foreach (var coinRack in CoinsInCoinRacks)		//Get each coin rack
			{
				foreach (var coin in coinRack)				//Get each coin in the current coin rack
				{
					expectedChange -= coin.Value;			//Subtract the coin's value from the expected change
				}
			}
			foreach (var coin in CoinsInStorage)			//Get each coin in the storage bin
			{
				expectedGross -= coin.Value;				//Subtract the coin's value from the expected amount
			}
			foreach (var popRack in PopInPopRacks)			//Make sure the PopCanRacks are empty
			{
				if (popRack.Count != 0)
					return false;
			}
			
			if ((expectedChange != 0) || (expectedGross != 0))	//If either the expected change or expected gross is not zero, return false
				return false;
			
			return true;	//Otherwise return true, meaning the extracted items match the expected items
		}
		
		
		public VendingMachineStoredContents UnloadVendingMachine(VendingMachine vm)
        {
            var storedContents = new VendingMachineStoredContents();

            foreach (var coinRack in vm.CoinRacks)
            {
                storedContents.CoinsInCoinRacks.Add(coinRack.Unload());
            }
            storedContents.PaymentCoinsInStorageBin.AddRange(vm.StorageBin.Unload());
            foreach (var popCanRack in vm.PopCanRacks)
            {
                storedContents.PopCansInPopCanRacks.Add(popCanRack.Unload());
            }

            return storedContents;
        }
        public int CoinRackSum(List<List<Coin>> rackList)
        {
            int RackCoinSum = 0;
            foreach (List<Coin> cr in rackList)
            {
                foreach (Coin c in cr)
                {
                    RackCoinSum += c.Value;
                }
            }
            return RackCoinSum;
        }
        public int StorageSum(List<Coin> coinList)
        {
            int StorageCoinSum = 0;
            foreach (Coin c in coinList)
            {
                StorageCoinSum += c.Value;
            }
            return StorageCoinSum;
        }
        public PopCan[] PopRackList(List<List<PopCan>> popRack)
        {
            List<PopCan> popTypeList = new List<PopCan>();
            foreach (List<PopCan> lpc in popRack)
            {
                if (lpc.Count != 0)
                {
                    popTypeList.Add(lpc[0]);
                }
            }
            return popTypeList.ToArray();
        }
        public int DeliverySum(IDeliverable[] objectList)
        {
            int sum = 0;
            foreach (IDeliverable id in objectList)
            {
                if (id.GetType() == typeof(Coin))
                {
                    sum += Convert.ToInt32(id.ToString());
                }
            }
            return sum;
        }
        public PopCan[] DeliveryPops(IDeliverable[] objectList)
        {
            List<PopCan> popCanList = new List<PopCan>();
            foreach (IDeliverable id in objectList)
            {
                if (id.GetType() == typeof(PopCan))
                {
                    popCanList.Add(new PopCan(id.ToString()));
                }
            }
            return popCanList.ToArray();
        }
    }
	
	[TestClass]
    public class BadTests
	{
		[TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void U01_bad_configure_before_construct(){
			List<VendingMachine> vendingMachines = new List<VendingMachine>();	//Make a list of vending machines
			int vmIndex = 0;													//The machine that will be called
			
			//Configure the vending machine (before construct)
			List<String> popNames = new List<String>(new String[]{"Coke", "water", "stuff"});
			List<int> popPrices = new List<int>(new int[]{250, 250, 205});
			vendingMachines[vmIndex].Configure(popNames, popPrices);	//EXCEPTION should be thrown
			
			//Create a vending machine (Should not reach)
			int[] coinKinds = new int[]{5, 10, 25, 100};						//Make the appropriate list of coin kinds
			
			vendingMachines.Add(new VendingMachine(coinKinds, 3, 10, 10, 10));	//Create the vending machine
			new VendingMachineLogic(vendingMachines[vmIndex]);					//Create a corresponding logic object
			
			//Load coins
			vendingMachines[vmIndex].CoinRacks[0].LoadCoins(new List<Coin>(new Coin[]{new Coin(5)}));
			vendingMachines[vmIndex].CoinRacks[1].LoadCoins(new List<Coin>(new Coin[]{new Coin(10)}));
			vendingMachines[vmIndex].CoinRacks[2].LoadCoins(new List<Coin>(new Coin[]{new Coin(25), new Coin(25)}));
			vendingMachines[vmIndex].CoinRacks[3].LoadCoins(new List<Coin>(new Coin[]{}));
			
			//Load pops
			vendingMachines[vmIndex].PopCanRacks[0].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("Coke")}));
			vendingMachines[vmIndex].PopCanRacks[1].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("water")}));
			vendingMachines[vmIndex].PopCanRacks[2].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("stuff")}));
			
			//Unload and check teardown
			int expectedChangeInCoinRacks = 65;
			int expectedChangeInStorageBin = 0;
			Dictionary<string, int> expectedPopInPopRacks = new Dictionary<string, int>();
			expectedPopInPopRacks["Coke"] = 1;
			expectedPopInPopRacks["water"] = 1;
			expectedPopInPopRacks["stuff"] = 1;
			Assert.IsTrue(unloadAndCheck(vendingMachines[vmIndex], expectedChangeInCoinRacks, expectedChangeInStorageBin, expectedPopInPopRacks));
		}

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void U02_bad_costs_list(){
			int[] coinKinds = new int[]{5, 10, 25, 100};						//Make the appropriate list of coin kinds
			
			VendingMachine VM1 = new VendingMachine(coinKinds, 3, 10, 10, 10);	//Create the vending machine
			new VendingMachineLogic(VM1);										//Create a corresponding logic object
			
			//Configure the vending machine
			List<String> popNames = new List<String>(new String[]{"Coke", "water", "stuff"});
			List<int> popPrices = new List<int>(new int[]{250, 250, 0});	//EXCEPTION should be thrown
			VM1.Configure(popNames, popPrices);
			
			//Load coins
			VM1.CoinRacks[0].LoadCoins(new List<Coin>(new Coin[]{new Coin(5)}));
			VM1.CoinRacks[1].LoadCoins(new List<Coin>(new Coin[]{new Coin(10)}));
			VM1.CoinRacks[2].LoadCoins(new List<Coin>(new Coin[]{new Coin(25), new Coin(25)}));
			VM1.CoinRacks[3].LoadCoins(new List<Coin>(new Coin[]{}));
			
			//Load pops
			VM1.PopCanRacks[0].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("Coke")}));
			VM1.PopCanRacks[1].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("water")}));
			VM1.PopCanRacks[2].LoadPops(new List<PopCan>(new PopCan[]{new PopCan("stuff")}));
			
			//Unload VM and check teardown
			int expectedChangeInCoinRacks = 0;
			int expectedChangeInStorageBin = 0;
			Assert.IsTrue(unloadAndCheck(VM1, expectedChangeInCoinRacks, expectedChangeInStorageBin));
		}

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void U03_bad_names_list(){
			//Create the vending machine
			int[] coinKinds = new int[]{5, 10, 25, 100};						//Make the appropriate list of coin kinds
			
			VendingMachine VM1 = new VendingMachine(coinKinds, 3, 10, 10, 10);	//Create the vending machine
			new VendingMachineLogic(VM1);										//Create a corresponding logic object
			
			//Configure the vending machine
			List<String> popNames = new List<String>(new String[]{"Coke", "water"});
			List<int> popPrices = new List<int>(new int[]{250, 250});
			VM1.Configure(popNames, popPrices);	//EXCEPTION should be thrown
		}

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void U04_bad_non_unique_denomination(){
			//Create the vending machine
			int[] coinKinds = new int[]{1, 1};
			
			VendingMachine VM1 = new VendingMachine(coinKinds, 1, 10, 10, 10);	//EXCEPTION should be thrown
			new VendingMachineLogic(VM1);
		}

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void U05_bad_coin_kind(){
			//Create the vending machine
			int[] coinKinds = new int[]{0};
			
			VendingMachine VM1 = new VendingMachine(coinKinds, 1, 10, 10, 10);	//EXCEPTION should be thrown
			new VendingMachineLogic(VM1);
		}

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void U06_bad_button_number(){
			//Create the vending machine
			int[] coinKinds = new int[]{5, 10, 25, 100};
			
			VendingMachine VM1 = new VendingMachine(coinKinds, 3, 10, 10, 10);	//Had to add the three parameters for rack size.
			new VendingMachineLogic(VM1);
			
			//Press button
			VM1.SelectionButtons[3].Press();	//EXCEPTION should be thrown
		}

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void U07_bad_button_number_2(){
			//Create the vending machine
			int[] coinKinds = new int[]{5, 10, 25, 100};
			
			VendingMachine VM1 = new VendingMachine(coinKinds, 3, 10, 10, 10);	//Had to add the three parameters for rack size.
			new VendingMachineLogic(VM1);
			
			//Press button
			VM1.SelectionButtons[-1].Press();	//EXCEPTION should be thrown
		}

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void U08_bad_button_number_3(){
			//Create the vending machine
			int[] coinKinds = new int[]{5, 10, 25, 100};
			
			VendingMachine VM1 = new VendingMachine(coinKinds, 3, 10, 10, 10);	//Had to add the three parameters for rack size.
			new VendingMachineLogic(VM1);
			
			//Press button
			VM1.SelectionButtons[4].Press();	//EXCEPTION should be thrown
		}
		
		//Methods to check the deliveries and teardowns.
		private bool checkDelivery(IDeliverable[] deliveredItems, int expectedChange, Dictionary<string, int> expectedPop){			
			foreach (var item in deliveredItems)						//Get each item in the delivery chute
			{
				if (item is PopCan)										//If the item is a pop
				{
					if (expectedPop.ContainsKey(((PopCan) item).Name))	//Check that the pop type is expected
						expectedPop[((PopCan) item).Name] -= 1;			//If it is expected, reduce the expected amount
					else
						return false;									//If the pop type is not expected, return false
				}
				else if (item is Coin)									//If the item is a coin,
					expectedChange -= ((Coin) item).Value;				//Reduce the expected change amount by the coin's value
			}
			
			foreach (int amount in expectedPop.Values)	//Make sure that the amount of each expected pop is zero
			{
				if (amount != 0)
					return false;						//If any expected pop is not 0, return false
			}
			if (expectedChange != 0)
				return false;							//If the amount of expected change is not 0, return false
			
			return true;	//If false is never returned, then the delivery check passes
		}
		private bool checkDelivery(IDeliverable[] deliveredItems, int expectedChange){
			foreach (var item in deliveredItems)			//Get each item in the delivery chute
			{
				if (item is Coin)							//If the item is a coin, reduce the expected change amount
					expectedChange -= ((Coin) item).Value;
				else
					return false;							//If there is something else (PopCan), return false
			}
			
			if (expectedChange != 0)	//If the expected change is not 0 at this point, return false
				return false;
			
			return true;				//Otherwise return true
		}
		
		private bool unloadAndCheck(VendingMachine vm, int expectedChange, int expectedGross, Dictionary<string, int> expectedPop){
			List<List<Coin>> coinRacks = new List<List<Coin>>();
			List<Coin> coinsInStorage;
			List<List<PopCan>> popRacks = new List<List<PopCan>>();
			
			foreach (var coinRack in vm.CoinRacks)
			{
				coinRacks.Add(coinRack.Unload());
			}
			coinsInStorage = vm.StorageBin.Unload();
			foreach (var popRack in vm.PopCanRacks)
			{
				popRacks.Add(popRack.Unload());
			}
			
			return (checkTeardown(coinRacks, coinsInStorage, popRacks, expectedChange, expectedGross, expectedPop));
		}
		private bool unloadAndCheck(VendingMachine vm, int expectedChange, int expectedGross){
			List<List<Coin>> coinRacks = new List<List<Coin>>();
			List<Coin> coinsInStorage;
			List<List<PopCan>> popRacks = new List<List<PopCan>>();
			
			foreach (var coinRack in vm.CoinRacks)
			{
				coinRacks.Add(coinRack.Unload());
			}
			coinsInStorage = vm.StorageBin.Unload();
			foreach (var popRack in vm.PopCanRacks)
			{
				popRacks.Add(popRack.Unload());
			}
			
			return (checkTeardown(coinRacks, coinsInStorage, popRacks, expectedChange, expectedGross));
		}
		
		private bool checkTeardown(List<List<Coin>> CoinsInCoinRacks, List<Coin> CoinsInStorage, List<List<PopCan>> PopInPopRacks, int expectedChange, int expectedGross, Dictionary<string, int> expectedPop){
			foreach (var coinRack in CoinsInCoinRacks)		//Get each coin rack
			{
				foreach (var coin in coinRack)				//Get each coin in the current coin rack
				{
					expectedChange -= coin.Value;			//Subtract the coin's value from the expected change
				}
			}
			foreach (var coin in CoinsInStorage)			//Get each coin in the storage bin
			{
				expectedGross -= coin.Value;				//Subtract the coin's value from the expected amount
			}
			foreach (var popRack in PopInPopRacks)			//Get each pop rack
			{
				foreach (var pop in popRack)				//Get each pop in the current pop rack
				{
					if (expectedPop.ContainsKey(pop.Name))	//If the expected pop dictionary contains the pop currently selected
						expectedPop[pop.Name] -= 1;			//Decrease the number of pops that are expected of the specific kind
					else
						return false;						//If the current pop is not exptected, return false
				}
			}
			
			foreach (int amount in expectedPop.Values)			//Check that all the expected amounts are zero
			{
				if (amount != 0)								//If an amount is not 0, return false
					return false;
			}
			if ((expectedChange != 0) || (expectedGross != 0))	//If either the expected change or expected gross is not zero, return false
				return false;
			
			return true;	//Otherwise return true, meaning the extracted items match the expected items
		}
		private bool checkTeardown(List<List<Coin>> CoinsInCoinRacks, List<Coin> CoinsInStorage, List<List<PopCan>> PopInPopRacks, int expectedChange, int expectedGross){
			foreach (var coinRack in CoinsInCoinRacks)		//Get each coin rack
			{
				foreach (var coin in coinRack)				//Get each coin in the current coin rack
				{
					expectedChange -= coin.Value;			//Subtract the coin's value from the expected change
				}
			}
			foreach (var coin in CoinsInStorage)			//Get each coin in the storage bin
			{
				expectedGross -= coin.Value;				//Subtract the coin's value from the expected amount
			}
			foreach (var popRack in PopInPopRacks)			//Make sure the PopCanRacks are empty
			{
				if (popRack.Count != 0)
					return false;
			}
			
			if ((expectedChange != 0) || (expectedGross != 0))	//If either the expected change or expected gross is not zero, return false
				return false;
			
			return true;	//Otherwise return true, meaning the extracted items match the expected items
		}
	}
}
