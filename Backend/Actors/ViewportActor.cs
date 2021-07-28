using System.Threading.Channels;
using System.Threading.Tasks;
using Proto;

namespace Backend.Actors
{
    public class ViewportActor : IActor
    {
        private readonly Channel<Position> _positions;
        private readonly Viewport _viewport;

        public ViewportActor(Channel<Position> positions)
        {
            _positions = positions;
            _viewport = new Viewport();
        }

        public async Task ReceiveAsync(IContext context)
        {
            switch (context.Message)
            {
                case Position position:
                {
                    if (position.IsWithinViewport(_viewport))
                    {
                        await _positions.Writer.WriteAsync(position);
                    }

                    break;
                }
                case UpdateViewport updateViewport:
                    SetViewport(updateViewport.Viewport);
                    break;
            }
        }

        private void SetViewport(Viewport viewport)
        {
            _viewport.Lat1 = viewport.Lat1;
            _viewport.Lat2 = viewport.Lat2;
            _viewport.Lng1 = viewport.Lng1;
            _viewport.Lng2 = viewport.Lng2;
        }


    }
}