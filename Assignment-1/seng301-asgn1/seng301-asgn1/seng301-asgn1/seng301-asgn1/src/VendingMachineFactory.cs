using System.Collections;
using System.Collections.Generic;

using Frontend1;

namespace seng301_asgn1 {
    /// <summary>
    /// Represents the concrete virtual vending machine factory that you will implement.
    /// This implements the IVendingMachineFactory interface, and so all the functions
    /// are already stubbed out for you.
    /// 
    /// Your task will be to replace the TODO statements with actual code.
    /// 
    /// Pay particular attention to extractFromDeliveryChute and unloadVendingMachine:
    /// 
    /// 1. These are different: extractFromDeliveryChute means that you take out the stuff
    /// that has already been dispensed by the machine (e.g. pops, money) -- sometimes
    /// nothing will be dispensed yet; unloadVendingMachine is when you (virtually) open
    /// the thing up, and extract all of the stuff -- the money we've made, the money that's
    /// left over, and the unsold pops.
    /// 
    /// 2. Their return signatures are very particular. You need to adhere to this return
    /// signature to enable good integration with the other piece of code (remember:
    /// this was written by your boss). Right now, they return "empty" things, which is
    /// something you will ultimately need to modify.
    /// 
    /// 3. Each of these return signatures returns typed collections. For a quick primer
    /// on typed collections: https://www.youtube.com/watch?v=WtpoaacjLtI -- if it does not
    /// make sense, you can look up "Generic Collection" tutorials for C#.
    /// </summary>
    public class deliveryChute
    {
        public List<Deliverable> containedItems = new List<Deliverable>();
        public deliveryChute()
        {
        }
    }
    public class popChute
    {
        public string popChuteType { get; set; }
        public List<Pop> containedPops = new List<Pop>();
        public popChute()
        {
        }
    }
    public class coinChute
    {
        public int coinChuteType { get; set; }
        public List<Coin> containedCoins = new List<Coin>();
        public coinChute()
        {
        }
    }
    public class VendingMachine
    {
        public List<int> coinKinds = new List<int>();
        public int selectionButtonCount { get; set; }
        public List<string> popNames = new List<string>();
        public List<int> popCosts = new List<int>();
        public List<coinChute> coinChuteCollection = new List<coinChute>();
        public List<popChute> popChuteCollection = new List<popChute>();
        public deliveryChute deliveryChute = new deliveryChute();
        public int insertedCoinsCounter { get; set; }
        public List<Coin> moneyGenerated = new List<Coin>();
        public List<Coin> coinLimbo = new List<Coin>();

        public VendingMachine()
        {
        }

        public void setCoinChuteCollection(List<coinChute> input)
        {
            this.coinChuteCollection = input;
        }
        public List<coinChute> getCoinChuteCollection()
        {
            return this.coinChuteCollection;
        }
        public void setPopChuteCollection(List<popChute> input)
        {
            this.popChuteCollection = input;
        }
        public List<popChute> getPopChuteCollection()
        {
            return this.popChuteCollection;
        }

    }
    public class VendingMachineFactory : IVendingMachineFactory {
        public List<VendingMachine> machineCollection = new List<VendingMachine>();
        public int machineIndex = -1;

        public VendingMachineFactory() {
        }

        public int createVendingMachine(List<int> coinKinds, int selectionButtonCount) {
            VendingMachine vm = new VendingMachine();
            int coinCounter = 0;
            foreach (int i in coinKinds)
            {            
                coinChute chute = new coinChute();
                chute.coinChuteType = i;
                vm.getCoinChuteCollection().Add(chute);

                if (vm.coinKinds.Contains(i))
                {
                    throw new System.ArgumentException("Duplicate items in list", "coinKinds");
                }
                if (i <= 0)
                {
                    throw new System.ArgumentException("Negative or zero number in list", "coinKinds");
                }
                else
                {
                    vm.coinKinds.Add(i);
                }
                coinCounter++;
            }

            vm.selectionButtonCount = selectionButtonCount;
            machineCollection.Add(vm);
            machineIndex++;

            return machineIndex;
        }

        public void configureVendingMachine(int vmIndex, List<string> popNames, List<int> popCosts) {
            VendingMachine vm = machineCollection[vmIndex];
            if (popNames.Count != popCosts.Count)
            {
                throw new System.ArgumentException("Pop costs and names do not match");
            }
            else
            {
                vm.popNames = popNames;
            }

            int popCounter = 0;
            foreach (int c in popCosts)
            {
                popChute chute = new popChute();
                chute.popChuteType = popNames[popCounter];
                vm.getPopChuteCollection().Add(chute);
                if (c <= 0)
                {
                    throw new System.ArgumentException("Negative or zero number in list", "popCosts");
                }
                else
                {
                    vm.popCosts.Add(c);
                }
                popCounter++;
            }
        }

        public void loadCoins(int vmIndex, int coinKindIndex, List<Coin> coins) {
            VendingMachine vm = machineCollection[vmIndex];
            vm.getCoinChuteCollection()[coinKindIndex].containedCoins = coins;
            vm.getCoinChuteCollection()[coinKindIndex].coinChuteType = coins[0].Value;
        }

        public void loadPops(int vmIndex, int popKindIndex, List<Pop> pops) {
            VendingMachine vm = machineCollection[vmIndex];
            vm.getPopChuteCollection()[popKindIndex].containedPops = pops;
            vm.getPopChuteCollection()[popKindIndex].popChuteType = pops[0].Name;
        }

        public void insertCoin(int vmIndex, Coin coin) {
            VendingMachine vm = machineCollection[vmIndex];
            if (!vm.coinKinds.Contains(coin.Value))
            {
                vm.deliveryChute.containedItems.Add(coin);
            }
            else
            {
                vm.insertedCoinsCounter = vm.insertedCoinsCounter + coin.Value;
                vm.coinLimbo.Add(coin);
            }
        }

        public void pressButton(int vmIndex, int value) {
            VendingMachine vm = machineCollection[vmIndex];
            string selectedPop = vm.getPopChuteCollection()[value].popChuteType; // determines the name of the pop within the selected chute
            int price = vm.popCosts[vm.popNames.IndexOf(selectedPop)]; // determines the price of the pop selected
            if (vm.insertedCoinsCounter >= price & vm.getPopChuteCollection()[value].containedPops.Count > 0)
            { // if inserted coins exceeds the minimum required for the pop do the following
                foreach (Coin c in vm.coinLimbo)
                {
                    vm.moneyGenerated.Add(c);
                }
                vm.coinLimbo.Clear();
                int change = vm.insertedCoinsCounter - price;
                if (getCoinChuteContents(vmIndex) >= change)
                { // if the vending machine contains more coins than required for change return coins       
                    List<int> coinList = vm.coinKinds;
                    coinList.Sort();
                    coinList.Reverse(); // creates list of coin types in descending order
                    int chuteIndex = 0;
                    int returnCounter = 0; // keeps track of the coins returned so far
                    foreach (int c in coinList)
                    { // loops through each coin in the coin type list, from largest to smallest
                        int count = (int)(change / c); // calculates approximation of how many coins of type c should be returned
                        int i = 0;
                        foreach (coinChute cc in vm.getCoinChuteCollection())
                        {
                            if (cc.coinChuteType == c)
                            {
                                chuteIndex = vm.getCoinChuteCollection().IndexOf(cc);
                            }
                        }
                        while (i < count && vm.getCoinChuteCollection()[chuteIndex].containedCoins.Count > 0)
                        { // loops until said amount has been despensed or coins have run out in the chute
                            Coin returnedCoin = vm.getCoinChuteCollection()[chuteIndex].containedCoins[vm.getCoinChuteCollection()[chuteIndex].containedCoins.Count - 1];
                            vm.deliveryChute.containedItems.Add(returnedCoin); // adds coin of type c to delivery chute
                            vm.getCoinChuteCollection()[chuteIndex].containedCoins.RemoveAt(vm.getCoinChuteCollection()[chuteIndex].containedCoins.Count - 1); // removes coin from coin chute
                            i++; // increment i by 1
                            returnCounter = returnCounter + c; // add value of c to return counter
                            change = change - c;
                        }
                        if (change == 0)
                        {
                            break;
                        }
                    }
                }
                List<Pop> chuteContents = vm.getPopChuteCollection()[value].containedPops; // determines contents of chute
                vm.deliveryChute.containedItems.Add(chuteContents[chuteContents.Count - 1]); // adds last pop in chute to the delivery chute
                vm.getPopChuteCollection()[value].containedPops.RemoveAt(vm.getPopChuteCollection()[value].containedPops.Count - 1); // removes last item in pop chute
            }
        }
        public int getCoinChuteContents(int vmIndex)
        {
            int total = 0;
            VendingMachine vm = machineCollection[vmIndex];
            foreach (coinChute cc in vm.getCoinChuteCollection())
            {
                foreach (Coin c in cc.containedCoins)
                {
                    total = total + c.Value;
                }
            }
            return total;
        }

        public List<Deliverable> extractFromDeliveryChute(int vmIndex)
        {
            VendingMachine vm = machineCollection[vmIndex];
            List<Deliverable> returnedItems = new List<Deliverable>();        
            foreach (Deliverable d in vm.deliveryChute.containedItems)
            {
                returnedItems.Add(d);
            }
            vm.deliveryChute.containedItems.Clear();
            return returnedItems;
        }

        public List<IList> unloadVendingMachine(int vmIndex)
        {
            VendingMachine vm = machineCollection[vmIndex];
            List<Coin> changeCoins = new List<Coin>();
            foreach (coinChute cc in vm.coinChuteCollection)
            {
                foreach (Coin c in cc.containedCoins)
                {
                    changeCoins.Add(c);
                }
            }
            List<Coin> moneyGenerated = new List<Coin>();
            foreach (Coin c in vm.moneyGenerated)
            {
                moneyGenerated.Add(c);
            }
            List<Pop> unsoldPops = new List<Pop>();
            foreach (popChute pc in vm.popChuteCollection)
            {
                foreach (Pop p in pc.containedPops)
                {
                    unsoldPops.Add(p);
                }
            }
            return new List<IList>()
            {
                changeCoins,
                moneyGenerated,
                unsoldPops };
            }
    }
}