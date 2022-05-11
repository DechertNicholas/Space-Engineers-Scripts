const string PistonName = "Sensor Piston";
const string SensorName = "Sensor";
const float PistonVelocity = 1.5f;

List<IMyPistonBase> Pistons;
List<IMySensorBlock> Sensors;
Dictionary<IMyPistonBase, List<IMySensorBlock>> PistonSensorMap = new Dictionary<IMyPistonBase, List<IMySensorBlock>>();

public Program()
{
    Pistons = GetBlockList<IMyPistonBase>(PistonName);
    Sensors = GetBlockList<IMySensorBlock>(SensorName);
    MapPistonsToSensors();

    Runtime.UpdateFrequency = UpdateFrequency.Update1;
}

public void Main(string argument, UpdateType updateSource)
{
    Pistons.ForEach(p => p.Velocity = AggregateDetectedEntities(PistonSensorMap[p]).Count > 0 ? -PistonVelocity : PistonVelocity);
}

public void MapPistonsToSensors()
{
    foreach(var piston in Pistons)
    {
        PistonSensorMap.Add(piston, new List<IMySensorBlock>());

        foreach(var sensor in Sensors)
        {
            if (piston.TopGrid.EntityId == sensor.CubeGrid.EntityId) PistonSensorMap[piston].Add(sensor);
        }

        if (PistonSensorMap[piston].Count == 0) throw new Exception($"Piston named \"{piston.CustomName}\" has no sensor attached.");
    }
}

public List<T> GetBlockList<T>(string name) where T : IMyTerminalBlock
{
    var result = new List<T>();
    var blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.GetBlockGroupWithName(name)?.GetBlocks(blocks);

    if (blocks.Count > 0)
    {
        foreach(var block in blocks) result.Add((T)block);
    }
    else
    {
        var block = (T) GridTerminalSystem.GetBlockWithName(name);
        if (block == null) throw new Exception($"Block or block group named \"{name}\" not found.");
        result = new List<T>() { block };
    }

    return result;
}

public List<MyDetectedEntityInfo> AggregateDetectedEntities(List<IMySensorBlock> sensorBlocks)
{
    var result = new List<MyDetectedEntityInfo>();
    var entities = new List<MyDetectedEntityInfo>();

    foreach(var sensor in sensorBlocks)
    {
        if (sensor == null) continue;

        sensor.DetectedEntities(entities);
        result.AddRange(entities);
    }

    return result;
}