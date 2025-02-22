using System;
using System.Collections.Generic;

public class DirtyChunkSorter : IComparer<Chunk> {
    private Player player;
    private Frustum frustum;
    private long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public DirtyChunkSorter(Player player, Frustum frustum) {
        this.player = player;
        this.frustum = frustum;
    }

    public int Compare(Chunk c0, Chunk c1) {
        bool i0 = this.frustum.isVisible(c0.aabb);
        bool i1 = this.frustum.isVisible(c1.aabb);
        if (i0 && !i1) {
            return -1;
        } else if (i1 && !i0) {
            return 1;
        } else {
            int t0 = (int)((this.now - c0.dirtiedTime) / 2000L);
            int t1 = (int)((this.now - c1.dirtiedTime) / 2000L);
            if (t0 < t1) {
                return -1;
            } else if (t0 > t1) {
                return 1;
            } else {
                return c0.distanceToSqr(this.player) < c1.distanceToSqr(this.player) ? -1 : 1;
            }
        }
    }
}
