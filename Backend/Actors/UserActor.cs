namespace Backend.Actors;

public delegate Task SendPositionBatch(PositionBatch batch);

public delegate Task SendNotification(Notification notification);

public class UserActor : IActor
{
    const int PositionBatchSize = 10;

    private readonly SendPositionBatch _sendPositionBatch;
    private readonly SendNotification _sendNotification;
    private readonly Viewport _viewport;
    private readonly List<Position> _positions = new(PositionBatchSize);
    private EventStreamSubscription<object> _notificationSubscription;
    private EventStreamSubscription<object> _positionSubscription;

    public UserActor(SendPositionBatch sendPositionBatch, SendNotification sendNotification)
    {
        _sendPositionBatch = sendPositionBatch;
        _sendNotification = sendNotification;
        _viewport = new Viewport();
    }

    public async Task ReceiveAsync(IContext context)
    {
        switch (context.Message)
        {
            case Started:
                SubscribeToEvents(context.System, context.Self);
                break;

            case Position position:
                await OnPosition(position);
                break;

            case Notification notification:
                await _sendNotification(notification);
                break;

            case UpdateViewport updateViewport:
                _viewport.MergeFrom(updateViewport.Viewport);
                break;

            case Stopping:
                UnsubscribeEvents(context);
                break;
        }
    }


    private void SubscribeToEvents(ActorSystem system, PID self)
    {
        // do not try to process the events in the handler, send to self instead
        // to avoid concurrency issues

        _notificationSubscription =
            system.EventStream.Subscribe<Notification>(
                notification => system.Root.Send(self, notification));
        _positionSubscription =
            system.EventStream.Subscribe<Position>(
                position => system.Root.Send(self, position));
    }

    private void UnsubscribeEvents(IContext context)
    {
        context.System.EventStream.Unsubscribe(_positionSubscription);
        _positionSubscription = null;
        context.System.EventStream.Unsubscribe(_notificationSubscription);
        _notificationSubscription = null;
    }


    private async Task OnPosition(Position position)
    {
        if (position.IsWithinViewport(_viewport))
        {
            _positions.Add(position);
            if (_positions.Count >= PositionBatchSize)
            {
                await _sendPositionBatch(new PositionBatch { Positions = { _positions } });
                _positions.Clear();
            }
        }
    }
}

public class UpdateViewport
{
    public Viewport Viewport { get; set; }
}