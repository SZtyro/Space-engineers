//---------------------| Config |------------------------------------
string collectorsGroupName = "Zbieracze";
string containersGroupName = "Magazyn";
string othersContainerName = "Reszta";
//-------------------------------------------------------------------
IMyInventory othersInventory;
List<IMyInventory> collectorInventories = new List<IMyInventory>();
List<IMyCargoContainer> containers = new List<IMyCargoContainer>();

public Program()
{
    
    othersInventory = (GridTerminalSystem.GetBlockWithName(othersContainerName) as IMyCargoContainer).GetInventory();
    GridTerminalSystem.GetBlockGroupWithName(containersGroupName).GetBlocksOfType(containers);

    List<IMyCargoContainer> collectors = new List<IMyCargoContainer>();
    GridTerminalSystem.GetBlockGroupWithName(collectorsGroupName).GetBlocksOfType(collectors);
    Echo("Znaleziono zbieraczy: " + collectors.Count);
    foreach(IMyCargoContainer collector in collectors){
        collectorInventories.Add(collector.GetInventory(0));
    }

    List<IMyRefinery> refineries = new List<IMyRefinery>();
    GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refineries);
    Echo("Znaleziono rafinerii: " + refineries.Count);
    foreach(IMyRefinery refinery in refineries){
        collectorInventories.Add(refinery.OutputInventory);
    }

    List<IMyAssembler> assemblers = new List<IMyAssembler>();
    GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers);
    Echo("Znaleziono stacji montażowych: " + assemblers.Count);
    foreach(IMyAssembler assembler in assemblers){
        collectorInventories.Add(assembler.OutputInventory);
    }
}

public void Main(string argument, UpdateType updateSource)
{
    foreach(IMyInventory inv in collectorInventories){
        if(inv.IsItemAt(0)){
            MyInventoryItem item = (MyInventoryItem)inv.GetItemAt(0);
            Echo("Aktualnie sortowany item: " + item.Type.SubtypeId);
            GetDestinationInventory(item).TransferItemFrom(inv, 0);
        }
    }

}

public IMyInventory GetDestinationInventory(MyInventoryItem item)
{
    IMyInventory destination = null;
    foreach(IMyCargoContainer container in containers){
        string[] containerItems = container.CustomName.Trim().Split(',');
        if(containerItems.Contains(item.Type.SubtypeId)){
            Echo("Cel sortowania: " + container.CustomName);
            destination = container.GetInventory();
            break;
        }
    }
    if(destination == null){
        Echo("Cel sortowania: " + othersContainerName);
        destination = othersInventory;
    }
        

    return destination;
}
