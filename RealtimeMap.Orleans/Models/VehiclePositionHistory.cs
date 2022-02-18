using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Models;

public class VehiclePositionHistory
{
    private const int Capacity = 100;
        
    // linked list is used for fast first item removing and inserting in chronological order
    private readonly LinkedList<VehiclePosition> _positions = new();
        
    public IReadOnlyCollection<VehiclePosition> Positions => _positions;
        
    public void Add(VehiclePosition position)
    {
        var node = _positions.Last;

        // add and keep chronological order
        if (node == null || node.Value.Timestamp <= position.Timestamp)
        {
            _positions.AddLast(position);
        }
        else
        {
            while (node != null && node.Value.Timestamp > position.Timestamp) node = node.Previous;
            if (node == null)
                _positions.AddFirst(position);
            else
                _positions.AddBefore(node, position);
        }
            
        if (_positions.Count > Capacity)
            _positions.RemoveFirst();
    }
}