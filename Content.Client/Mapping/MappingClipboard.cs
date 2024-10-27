using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Map;
using Robust.Client.UserInterface;
using Robust.Client.Input;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client.Mapping
{
    public class Clipboard
    {
        [Dependency] protected readonly IUserInterfaceManager UserInterfaceManager = default!;
        [Dependency] private readonly IEntityManager _entityManager = default!;
        [Dependency] private readonly IInputManager _input = default!;
        [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
        [Dependency] private readonly EntityLookupSystem _lookup = default!;
        [Dependency] private readonly SharedMapSystem _map = default!;

        private readonly HashSet<EntityUid> _clipboard = new();
        private EntityCoordinates? _startPoint = null;
        private Box2? _clipboardRect = null;

        public bool IsCopying()
        {
            return _clipboardRect.HasValue;
        }

        public bool HandleStartCopy(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
        {
            if (IsCopying())
                return;

            if (UserInterfaceManager.CurrentlyHovered is not IViewportControl viewport ||
                _input.MouseScreenPosition is not { IsValid: true } position)
            {
                return;
            }

            _startPoint = coords;

            _clipboardRect = new Box2(_startPoint.X, _startPoint.Y, _startPoint.X, _startPoint.Y);
            return true;
        }

        public void HandleCancelCopy()
        {
            _startPoint = null;
            _clipboardRect = null;
            return;
        }
        public bool HandleEndCopy(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
        {
            if (!IsCopying())
                return ;

            if (UserInterfaceManager.CurrentlyHovered is not IViewportControl viewport ||
                _input.MouseScreenPosition is not { IsValid: true } position)
            {
                return;
            }

            _clipboardRect = new Box2(_startPoint.X, _startPoint.Y, coords.X, coords.Y);

            // get all the entities in the area (see _entityManager query AABB) EntityPrototype


            // get all the tiles in the area ContentTileDefinition
            // get all the decals in the area DecalPrototype
            // load all of them into the clipboard object with offset information.

            _startPoint = null;
            _clipboardRect = null;
            return true;
        }


        public void HandlePaste(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
        {
            // first, place all tiles
            // then place all entities
            // then place all decals
            return true;
        }
    }
}
