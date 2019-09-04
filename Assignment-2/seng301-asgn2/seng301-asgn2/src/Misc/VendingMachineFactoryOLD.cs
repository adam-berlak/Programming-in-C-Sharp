using System;
using System.Collections.Generic;
using Frontend2;
using Frontend2.Hardware;

public class VendingMachineFactory : IVendingMachineFactory {
    List<VendingMachine> vendingMachines;

    public int CreateVendingMachine(List<int> coinKinds, int selectionButtonCount, int coinRackCapacity, int popRackCapcity, int receptacleCapacity) {
        var index = this.vendingMachines.Count;
        this.vendingMachines.Add(new VendingMachine(coinKinds, selectionButtonCount));
        return index;
    }

    public void ConfigureVendingMachine(int vmIndex, List<string> popNames, List<int> popCosts) {
        // TODO: Implement
    }

    public void LoadCoins(int vmIndex, int coinKindIndex, List<Coin> coins) {
        // TODO: Implement
    }

    public void LoadPops(int vmIndex, int popKindIndex, List<PopCan> pops) {
        // TODO: Implement
    }

    public void InsertCoin(int vmIndex, Coin coin) {
        // TODO: Implement
    }

    public void PressButton(int vmIndex, int value) {
        // TODO: Implement
    }

    public List<IDeliverable> ExtractFromDeliveryChute(int vmIndex) {
        // TODO: Implement
        return new List<IDeliverable>();
    }

    public VendingMachineStoredContents UnloadVendingMachine(int vmIndex) {
        // TODO: Implement
        return new VendingMachineStoredContents();
    }
}