using System.Collections.Generic;

public class RotationValues
{
    private readonly Dictionary<RoomForm, List<(RoomDirection[], float, (RoomDirection, RoomDirection)[])>>
        rotationConfigs =
            new Dictionary<RoomForm, List<(RoomDirection[], float, (RoomDirection, RoomDirection)[])>>()
            {
                {
                    RoomForm.U, new List<(RoomDirection[], float, (RoomDirection, RoomDirection)[])>
                    {
                        (new[] { RoomDirection.UP }, 270, new[] { (RoomDirection.RIGHT, RoomDirection.UP) }),
                        (new[] { RoomDirection.LEFT }, 180, new[] { (RoomDirection.RIGHT, RoomDirection.LEFT) }),
                        (new[] { RoomDirection.DOWN }, 90, new[] { (RoomDirection.RIGHT, RoomDirection.DOWN) })
                    }
                },
                {
                    RoomForm.I, new List<(RoomDirection[], float, (RoomDirection, RoomDirection)[])>
                    {
                        (new[] { RoomDirection.LEFT }, 90,
                            new[]
                            {
                                (RoomDirection.UP, RoomDirection.RIGHT), (RoomDirection.DOWN, RoomDirection.LEFT)
                            })
                    }
                },
                {
                    RoomForm.L, new List<(RoomDirection[], float, (RoomDirection, RoomDirection)[])>
                    {
                        (new[] { RoomDirection.DOWN, RoomDirection.LEFT }, 90,
                            new[]
                            {
                                (RoomDirection.DOWN, RoomDirection.LEFT), (RoomDirection.RIGHT, RoomDirection.DOWN)
                            }),
                        (new[] { RoomDirection.UP, RoomDirection.LEFT }, 180,
                            new[]
                            {
                                (RoomDirection.DOWN, RoomDirection.UP), (RoomDirection.RIGHT, RoomDirection.LEFT)
                            }),
                        (new[] { RoomDirection.UP, RoomDirection.RIGHT }, 270,
                            new[]
                            {
                                (RoomDirection.DOWN, RoomDirection.RIGHT), (RoomDirection.RIGHT, RoomDirection.UP)
                            })
                    }
                },
                {
                    RoomForm.T, new List<(RoomDirection[], float, (RoomDirection, RoomDirection)[])>
                    {
                        (new[] { RoomDirection.DOWN, RoomDirection.LEFT, RoomDirection.RIGHT }, 90,
                            new[]
                            {
                                (RoomDirection.UP, RoomDirection.RIGHT), (RoomDirection.RIGHT, RoomDirection.DOWN),
                                (RoomDirection.DOWN, RoomDirection.LEFT)
                            }),
                        (new[] { RoomDirection.UP, RoomDirection.DOWN, RoomDirection.LEFT }, 180,
                            new[]
                            {
                                (RoomDirection.UP, RoomDirection.DOWN), (RoomDirection.RIGHT, RoomDirection.LEFT),
                                (RoomDirection.DOWN, RoomDirection.UP)
                            }),
                        (new[] { RoomDirection.UP, RoomDirection.RIGHT, RoomDirection.LEFT }, 270,
                            new[]
                            {
                                (RoomDirection.UP, RoomDirection.LEFT), (RoomDirection.RIGHT, RoomDirection.UP),
                                (RoomDirection.DOWN, RoomDirection.RIGHT)
                            })
                    }
                }
            };

    public IReadOnlyDictionary<RoomForm, List<(RoomDirection[], float, (RoomDirection, RoomDirection)[])>>
        RotationConfigs => rotationConfigs;
}