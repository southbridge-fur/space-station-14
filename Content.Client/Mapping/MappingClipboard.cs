using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Map;
using Robust.Client.UserInterface;
using Robust.Client.Input;
using Robust.Client.Graphics;
using Robust.Client.Placement;
using Robust.Shared.Prototypes;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client.Mapping
{
    public sealed partial class Clipboard
    {
        [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
        [Dependency] private readonly IEntityManager _entityManager = default!;
        [Dependency] private readonly IInputManager _input = default!;
        [Dependency] private readonly IPlacementManager _placement = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
        [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
        [Dependency] private readonly EntityLookupSystem _lookup = default!;
        [Dependency] private readonly SharedMapSystem _map = default!;

        private HashSet<EntityUid> _clipboard = new HashSet<EntityUid>();
        private EntityCoordinates _startPoint = EntityCoordinates.Invalid;
        private Box2 _clipboardRect = Box2.Empty;

        public Clipboard()
        {
            Reset();
            _clipboard = new HashSet<EntityUid>();
        }

        public bool IsCopying()
        {
            return _clipboardRect.IsEmpty();
        }

        public bool HandleStartCopy(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
        {
            if (IsCopying())
                return false;

            if (_placement is not { } ||
                _placement.CurrentMode is not { } currentMode)
            {
                return false;
            }

            _startPoint = currentMode.MouseCoords;

            _clipboardRect = new Box2(_startPoint.X, _startPoint.Y, _startPoint.X, _startPoint.Y);
            return true;
        }

        public bool HandleCancelCopy()
        {
            Reset();
            return true;
        }

        private void Reset()
        {
            _startPoint = EntityCoordinates.Invalid;
            _clipboardRect = Box2.Empty;
        }

        public bool HandleEndCopy(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
        {
            if (!IsCopying())
                return false;


            if (_placement is not { } ||
                _placement.CurrentMode is not { } currentMode)
            {
                return false;
            }

            _clipboardRect = new Box2(
                _startPoint.X,
                _startPoint.Y,
                currentMode.MouseCoords.X,
                currentMode.MouseCoords.Y
            );

            var mapId = _transformSystem.GetMapId(coords);

            _clipboard = _lookup.GetEntitiesIntersecting(mapId, _clipboardRect);

            Reset();

            return true;
        }


        public bool HandlePaste(ICommonSession? session, EntityCoordinates coords, EntityUid uid)
        {
            // first, place all tiles
            // then place all entities
            // then place all decals
            return true;
        }
    }
}
