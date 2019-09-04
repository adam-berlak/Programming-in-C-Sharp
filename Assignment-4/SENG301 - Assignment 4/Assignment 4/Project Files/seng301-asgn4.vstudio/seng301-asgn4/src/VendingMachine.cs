using System;
using System.Collections.Generic;
using Frontend4;
using Frontend4.Hardware;


/**
 * Represents vending machines, fully configured and with all software
 * installed.
 * 
 */
public class VendingMachine {

    private HardwareFacade hardwareFacade;
    public HardwareFacade Hardware {
        get {
            return this.hardwareFacade;
        }
    }

    /**
     * Creates a standard arrangement for the vending machine. All the
     * components are created and interconnected. The hardware is initially
     * empty. The product kind names and costs are initialized to &quot; &quot;
     * and 1 respectively.
     * 
     * @param coinKinds
     *            The values (in cents) of each kind of coin. The order of the
     *            kinds is maintained. One coin rack is produced for each kind.
     *            Each kind must have a unique, positive value.
     * @param selectionButtonCount
     *            The number of selection buttons on the machine. Must be
     *            positive.
     * @param coinRackCapacity
     *            The maximum capacity of each coin rack in the machine. Must be
     *            positive.
     * @param productRackCapacity
     *            The maximum capacity of each product rack in the machine. Must
     *            be positive.
     * @param receptacleCapacity
     *            The maximum capacity of the coin receptacle, storage bin, and
     *            delivery chute. Must be positive.
     * @throws IllegalArgumentException
     *             If any of the arguments is null, or the size of productCosts
     *             and productNames differ.
     */
    public VendingMachine(Cents[] coinKinds, int selectionButtonCount, int coinRackCapacity, int productRackCapacity, int receptacleCapacity) {
	    this.hardwareFacade = new HardwareFacade(coinKinds, selectionButtonCount, coinRackCapacity, productRackCapacity, receptacleCapacity);

        CommunicationFacade cf = new CommunicationFacade(this.hardwareFacade);
        ProductFacade prf = new ProductFacade(this.hardwareFacade);
        PaymentFacade paf = new PaymentFacade(this.hardwareFacade);

        BuisnessRule br = new BuisnessRule(cf, prf, paf);
    }

   public void Configure(List<ProductKind> productKinds)
   {
        this.hardwareFacade.Configure(productKinds);
   }
}
public class BuisnessRule
{
    CommunicationFacade cf;
    ProductFacade prf;
    PaymentFacade paf;

    public BuisnessRule(CommunicationFacade cf, ProductFacade prf, PaymentFacade paf)
    {
        this.cf = cf;
        this.prf = prf;
        this.paf = paf;

        this.cf.SelectionMade += new EventHandler(SelectionMade);
        this.cf.ProductVended += new EventHandler(productVendedHandler);
        this.cf.OutOfStock += new EventHandler(outOfStockHandler);

        this.paf.FundsSufficient += new EventHandler(ReturnToDelivery);
        this.paf.FundsInsufficient += new EventHandler(FundsInsufficiantHandler);
    }

    private void SelectionMade(object sender, EventArgs e)
    {
        this.paf.checkFunds(cf.productCost);
    }

    private void ReturnToDelivery(object sender, EventArgs e)
    {
        this.paf.returnChange();
        this.prf.deliverProduct(cf.buttonPressed);  
    }

    private void FundsInsufficiantHandler(object sender, EventArgs e)
    {
        if (this.paf.availableFunds.Value + this.paf.credit.Value >= this.paf.productCost.Value)
        {
            this.prf.deliverProduct(cf.buttonPressed);
            this.paf.ProccessWithCredit();
        }
    }

    private void productVendedHandler(object sender, EventArgs e){}
    private void outOfStockHandler(object sender, EventArgs e) { }
}
public class PaymentFacade
{
    public HardwareFacade hf;
    public Cents availableFunds;
    public Boolean sufficiantFunds;
    public Cents productCost;
    public Cents changeNeeded;
    public Cents credit;
    public List<Coin> insertedCoins;

    public event EventHandler FundsSufficient;
    public event EventHandler FundsInsufficient;

    public PaymentFacade(HardwareFacade hf)
    {
        this.hf = hf;

        this.productCost = new Cents(0);
        this.availableFunds = new Cents(0);
        this.changeNeeded = new Cents(0);
        this.credit = new Cents(0);

    hf.CoinSlot.CoinAccepted += new EventHandler<CoinEventArgs>(insertSuccessful);
    }
    public void LoadCoins(int[] coinCounts)
    {
        hf.LoadCoins(coinCounts);
    }
    public void InsertFunds(Cents ammount)
    {
        this.credit = this.credit + ammount;
    }
    public void InsertFunds(Coin[] ammount)
    {
        hf.CoinSlot.CoinAccepted += new EventHandler<CoinEventArgs>(insertSuccessful);
        foreach (Coin c in ammount)
        {
            hf.CoinSlot.AddCoin(c);
            this.insertedCoins.Add(c); 
        }
    }
    private void insertSuccessful(object sender, CoinEventArgs e)
    {
        this.availableFunds += e.Coin.Value;
    }
    public void checkFunds(Cents productCost)
    {
        this.productCost = productCost;
        
        if (this.availableFunds >= productCost)
        {
            this.sufficiantFunds = true;
            this.changeNeeded = new Cents(availableFunds.Value - productCost.Value);
            this.FundsSufficient(this, new EventArgs());
        }
        else
        {         
            this.sufficiantFunds = false;
            this.FundsInsufficient(this, new EventArgs());          
        }    
    }

    public void returnChange()
    {
        this.hf.CoinReceptacle.StoreCoins(); 

        List<int> coinKinds = new List<int>();
        for (int i = 0; i < this.hf.CoinRacks.Length; i++)
        {          
            coinKinds.Add(this.hf.GetCoinKindForCoinRack(i).Value);
        }
        coinKinds.Sort(); coinKinds.Reverse();

        foreach (int i in coinKinds)
        {
            int count = (int)(changeNeeded.Value / i);
            int n = 0;
            int counter = 0;
            while (n < count)
            {
                n++;
                try
                {
                    this.hf.GetCoinRackForCoinKind(new Cents(i)).ReleaseCoin();
                    counter++;
                }
                catch
                {
                    System.Console.WriteLine("Ran out of coins");
                }
            }
            changeNeeded = new Cents(changeNeeded.Value - (i * counter));
        }
        this.credit = changeNeeded;

        this.availableFunds = new Cents(0);
        this.insertedCoins = new List<Coin>();
        this.changeNeeded = new Cents(0);
        this.sufficiantFunds = false;
    }

    public void ProccessWithCredit()
    {
        this.hf.CoinReceptacle.StoreCoins();

        this.credit = new Cents(this.credit.Value - (this.productCost.Value - this.availableFunds.Value));
        this.availableFunds = new Cents(0);
        this.insertedCoins = new List<Coin>();
        this.changeNeeded = new Cents(0);
        this.sufficiantFunds = false;
    }
}
public class ProductFacade
{
    public HardwareFacade hf;
    public ProductKind productName;

    Dictionary<SelectionButton, int> selectionButtonToIndex;

    public ProductFacade(HardwareFacade hf)
    {
        this.hf = hf;

        this.selectionButtonToIndex = new Dictionary<SelectionButton, int>();
        for (int i = 0; i < this.hf.SelectionButtons.Length; i++)
        {
            this.selectionButtonToIndex[this.hf.SelectionButtons[i]] = i;
        }
    }
    public void deliverProduct(ISelectionButton button)
    {

        var index = this.selectionButtonToIndex[(SelectionButton)button];
        this.productName = this.hf.ProductKinds[index];
        this.hf.ProductRacks[index].DispenseProduct();
    }
    public void loadProduct(int[] productCounts)
    {
        hf.LoadProducts(productCounts);
    }
}
public class CommunicationFacade
{
    public HardwareFacade hf;
    public PaymentFacade pf;
    Dictionary<SelectionButton, int> selectionButtonToIndex;
    public ProductKind productName;
    public Cents productCost;
    public ISelectionButton buttonPressed;

    public event EventHandler SelectionMade;
    public event EventHandler OutOfStock;
    public event EventHandler ProductVended;

    public CommunicationFacade(HardwareFacade hf)
    {
        this.hf = hf;
        foreach (ProductRack pr in this.hf.ProductRacks)
        {
            pr.ProductRemoved += new EventHandler<ProductEventArgs>(ProductVendedHandler);
            pr.ProductRackEmpty += new EventHandler(OutOfStockHandler);
        }

        this.selectionButtonToIndex = new Dictionary<SelectionButton, int>();
        for (int i = 0; i < this.hf.SelectionButtons.Length; i++)
        {
            this.hf.SelectionButtons[i].Pressed += new EventHandler(SelectionMadeHandler);
            this.selectionButtonToIndex[this.hf.SelectionButtons[i]] = i;
        }
    }
    public void Press(int buttonNo)
    {
        hf.SelectionButtons[buttonNo].Press();
    }
    public void SelectionMadeHandler(object sender, EventArgs e) {
        
        var index = this.selectionButtonToIndex[(SelectionButton)sender];
        this.productName = this.hf.ProductKinds[index];
        this.productCost = this.hf.ProductKinds[index].Cost;
        this.buttonPressed = (ISelectionButton) sender;

        this.SelectionMade(this, new EventArgs());   
    }

    public void ProductVendedHandler(object sender, EventArgs e) { this.ProductVended(this, new EventArgs()); }
    public void OutOfStockHandler(object sender, EventArgs e) { this.OutOfStock(this, new EventArgs()); }
}


