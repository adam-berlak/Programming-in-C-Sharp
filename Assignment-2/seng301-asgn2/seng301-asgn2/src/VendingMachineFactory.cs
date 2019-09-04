using System;
using System.Collections.Generic;
using Frontend2;
using Frontend2.Hardware;

public class VendingMachineFactory : IVendingMachineFactory {
    List<VendingMachine> vendingMachines = new List<VendingMachine>();
    List<List<int>> coinTypes = new List<List<int>>();
    List<List<string>> popTypes = new List<List<string>>();

    public int CreateVendingMachine(List<int> coinKinds, int selectionButtonCount, int coinRackCapacity, int popRackCapcity, int receptacleCapacity) {
        // var index = 0;
        System.Console.WriteLine("test");
        var index = this.vendingMachines.Count;

        int[] coinKindsArray = coinKinds.ToArray();
        this.vendingMachines.Add(new VendingMachine(coinKindsArray, selectionButtonCount, coinRackCapacity, popRackCapcity, receptacleCapacity));
        this.coinTypes.Add(coinKinds);

        return index;
    }

    public void ConfigureVendingMachine(int vmIndex, List<string> popNames, List<int> popCosts) {
        this.vendingMachines[vmIndex].Configure(popNames, popCosts);
        popTypes.Add(popNames);
    }

    public void LoadCoins(int vmIndex, int coinKindIndex, List<Coin> coins) {

        VendingMachine vm = vendingMachines[vmIndex];
        foreach (Coin c in coins)
        {
            if (c.Value <= 0)
            {
                throw new Exception("Each count must not be negative");
            }
        }
        vm.CoinRacks[coinKindIndex].LoadCoins(coins);
    }

    private void VendingMachineFactory_CoinAdded(object sender, CoinEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void LoadPops(int vmIndex, int popKindIndex, List<PopCan> pops) {

        VendingMachine vm = vendingMachines[vmIndex];
        foreach (PopCan p in pops)
        {
             vm.PopCanRacks[popKindIndex].AddPopCan(p);    
        }
    }

    public VendingMachineStoredContents UnloadVendingMachine(int vmIndex) {
        VendingMachineStoredContents contents = new VendingMachineStoredContents();

        foreach (CoinRack cr in this.vendingMachines[vmIndex].CoinRacks)
        {
            contents.CoinsInCoinRacks.Add(cr.Unload());
        }
        foreach (Coin c in this.vendingMachines[vmIndex].StorageBin.Unload())
        {
            contents.PaymentCoinsInStorageBin.Add(c);
        }
        foreach (PopCanRack pcr in this.vendingMachines[vmIndex].PopCanRacks)
        {
            contents.PopCansInPopCanRacks.Add(pcr.Unload());
        }

        return contents;
    }

    public List<IDeliverable> ExtractFromDeliveryChute(int vmIndex) {
        var deliveryChuteContentsArray = this.vendingMachines[vmIndex].DeliveryChute.RemoveItems();
        List<IDeliverable> deliveryChuteContents = new List<IDeliverable>(deliveryChuteContentsArray);
        return deliveryChuteContents;
    }

    public void InsertCoin(int vmIndex, Coin coin) {
        this.vendingMachines[vmIndex].CoinSlot.AddCoin(coin);     
    }


    public void PressButton(int vmIndex, int value) {
        int total = 0;
        this.vendingMachines[vmIndex].SelectionButtons[value].Press();
        List<Coin> popsInReceptical = this.vendingMachines[vmIndex].CoinReceptacle.Unload();
        foreach (Coin c in popsInReceptical)
        {
            total = total + c.Value;
        }
        this.vendingMachines[vmIndex].CoinReceptacle.LoadCoins(popsInReceptical);
        if (total >= this.vendingMachines[vmIndex].PopCanCosts[value])
        {
            this.vendingMachines[vmIndex].CoinReceptacle.StoreCoins(); // Do after customor has paid
            this.vendingMachines[vmIndex].PopCanRacks[value].DispensePopCan();
            int change = total - this.vendingMachines[vmIndex].PopCanCosts[value];
            List<int> coinKinds = this.coinTypes[vmIndex];
            coinKinds.Sort(); coinKinds.Reverse();
            foreach (int i in coinKinds)
            {
                int count = (int)(change / i);
                int n = 0;
                int counter = 0;
                while (n < count)
                {
                    n++;
                    try
                    {
                        this.vendingMachines[vmIndex].GetCoinRackForCoinKind(i).ReleaseCoin();
                        counter++;
                    }
                    catch
                    {
                        System.Console.WriteLine("Ran out of coins");
                    }                    
                }
                change = change - (i * counter);
            }        
            
        }    
    }
    void mc_buttonPressed(object sender, EventArgs e)
    {
        System.Console.WriteLine("Button pressed");
    }
}