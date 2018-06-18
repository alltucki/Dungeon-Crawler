using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/*
 * Fog of War algorithm heavily based on:
 * http://journal.stuffwithstuff.com/2015/09/07/what-the-hero-sees/
 */

public class fov_manager : MonoBehaviour
{

    public Tilemap fog_of_war;
    public int visual_range;
    private Vector2Int player;
    private Color visible, not_visible, prev_seen;
    private player_manager p_manager;
    private bool[,] has_seen;
    public bool[,] is_visible, transparent;
    public TileBase fog_base;

    // Use this for initialization
    void Start()
    {
        p_manager = GameObject.Find("Player Manager").GetComponent<player_manager>();

        visible = new Color(0, 0, 0, 0);
        not_visible = new Color(0, 0, 0, 1);
        prev_seen = new Color(0, 0, 0, .5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void reset_fog()
    {
        for(int x = 0; x < util_ref.m_gen.x_dim; x++)
        {
            for(int y = 0; y < util_ref.m_gen.y_dim; y++)
            {
                has_seen[x, y] = false;
                transparent[x, y] = util_ref.m_gen.is_floor[x, y];
            }
        }
    }

    public void get_fow()
    {
        fog_of_war = GameObject.Find("FoW").GetComponent<Tilemap>();
        fog_of_war.size = new Vector3Int(util_ref.m_gen.x_dim, util_ref.m_gen.y_dim, 0);
    }

    public void set_visible(Vector2Int position)
    {
        if (!is_valid(position))
        {
            Debug.LogWarning("Tried to set visible for position " + position.ToString());
            return;
        }
        fog_of_war.SetColor(new Vector3Int(position.x, position.y, 0), visible);
        has_seen[position.x, position.y] = true;
        is_visible[position.x, position.y] = true;
    }

    public void set_visible(int x, int y)
    {
        if(!is_valid(x,y))
        {
            Debug.LogWarning("Tried to set visible for position " + new Vector2Int(x, y).ToString());
            return;
        }
        fog_of_war.SetColor(new Vector3Int(x, y, 0), visible);
        has_seen[x, y] = true;
        is_visible[x, y] = true;
    }

    public void set_not_visible(Vector2Int position)
    {
        if (!is_valid(position))
        {
            Debug.LogWarning("Tried to set not visible for position " + position.ToString());
            return;
        }
        is_visible[position.x, position.y] = false;
        if (has_seen[position.x, position.y])
        {
            fog_of_war.SetColor(new Vector3Int(position.x, position.y, 0), prev_seen);
        }
        else
        {
            fog_of_war.SetColor(new Vector3Int(position.x, position.y, 0), not_visible);
        }

    }

    public void set_not_visible(int x, int y)
    {
        if (!is_valid(x, y))
        {
            Debug.LogWarning("Tried to set not visible for position " + new Vector2Int(x,y).ToString());
            return;
        }
        is_visible[x, y] = false;
        if (has_seen[x, y])
        {
            fog_of_war.SetColor(new Vector3Int(x, y, 0), prev_seen);
        }
        else
        {
            fog_of_war.SetColor(new Vector3Int(x, y, 0), not_visible);
        }

    }

    public void add_fog()
    {
        fog_of_war.BoxFill(new Vector3Int(0, 0, 0), fog_base, 0, 0, util_ref.m_gen.x_dim, util_ref.m_gen.y_dim);
        has_seen = new bool[util_ref.m_gen.x_dim, util_ref.m_gen.y_dim];
        is_visible = new bool[util_ref.m_gen.x_dim, util_ref.m_gen.y_dim];
        transparent = new bool[util_ref.m_gen.x_dim, util_ref.m_gen.y_dim];

        for (int x = 0; x < util_ref.m_gen.x_dim; x++)
        {
            for (int y = 0; y < util_ref.m_gen.y_dim; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                fog_of_war.SetTileFlags(pos, TileFlags.None);
                fog_of_war.SetColor(pos, prev_seen);
                has_seen[x, y] = false;
                is_visible[x, y] = false;
                transparent[x,y] = util_ref.m_gen.is_floor[x, y];
            }
        }
    }

    public bool is_valid(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= util_ref.m_gen.x_dim)
        {
            return false;
        }
        if (pos.y < 0 || pos.y >= util_ref.m_gen.y_dim)
        {
            return false;
        }
        return true;
    }

    public bool is_valid(int x, int y)
    {
        if (x < 0 || x >= util_ref.m_gen.x_dim)
        {
            return false;
        }
        if (y < 0 || y >= util_ref.m_gen.y_dim)
        {
            return false;
        }
        return true;
    }

    public void paint(int x, int y, Color color)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        util_ref.m_gen.floor_map.SetTileFlags(pos, TileFlags.None);
        util_ref.m_gen.floor_map.SetColor(pos, color);
        util_ref.m_gen.wall_map.SetTileFlags(pos, TileFlags.None);
        util_ref.m_gen.wall_map.SetColor(pos, color);
    }


    #region FOV algorithm

    //  Octant data
    //
    //    \ 1 | 2 /
    //   8 \  |  / 3
    //   -----+-----
    //   7 /  |  \ 4
    //    / 6 | 5 \
    //
    //  1 = NNW, 2 =NNE, 3=ENE, 4=ESE, 5=SSE, 6=SSW, 7=WSW, 8 = WNW

    /// <summary>
    /// Start here: go through all the octants which surround the player to
    /// determine which open cells are visible
    /// </summary>
    public void refresh_fog()
    {
        for (int x = 0; x < util_ref.m_gen.x_dim; x++)
        {
            for (int y = 0; y < util_ref.m_gen.y_dim; y++)
            {
                set_not_visible(x,y);
            }
        }

        player = p_manager.get_p_pos();
        for (int octant = 0; octant < 8; octant++)
        {
            ScanOctant(1, octant + 1, 1.0, 0.0);
        }

        set_visible(player);
    }

    /// <summary>
    /// Examine the provided octant and calculate the visible cells within it.
    /// </summary>
    /// <param name="pDepth">Depth of the scan</param>
    /// <param name="pOctant">Octant being examined</param>
    /// <param name="pStartSlope">Start slope of the octant</param>
    /// <param name="pEndSlope">End slope of the octance</param>
    protected void ScanOctant(int pDepth, int pOctant, double pStartSlope, double pEndSlope)
    {

        int visrange2 = visual_range * visual_range;
        int x = 0;
        int y = 0;

        switch (pOctant)
        {

            case 6: //nnw
                
                y = player.y - pDepth;
                if (y < 0) return;

                x = player.x - (int)(pStartSlope * (float)pDepth);
                if (x < 0) x = 0;

                while (GetSlope(x, y, player.x, player.y, false) >= pEndSlope)
                {
                    //paint(x, y, Color.white);
                    if (GetVisDistance(x, y, player.x, player.y) <= visrange2)
                    {
                        set_visible(x, y);
                        //TODO: CHANGE IS_FLOOR CHECK TO IS_TRANSPARENT CHECK,
                        //MODIFY IN MAP MANAGER AS NEEDED
                        if (is_valid(x, y) && !transparent[x, y]) //current cell blocked
                        {
                            if (x - 1 >= 0 && transparent[x - 1, y])
                            { //prior cell within range AND open...
                              //...incremenet the depth, adjust the endslope and recurse
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, player.x, player.y, false));
                            }
                        }
                        else
                        {

                            if (x - 1 >= 0 && !transparent[x - 1, y])
                            { //prior cell within range AND open...
                              //..adjust the startslope
                                pStartSlope = GetSlope(x - 0.5, y - 0.5, player.x, player.y, false);
                            }
                        }
                    }
                    x++;
                }
                x--;
                break;

            case 5: //nne
               
                y = player.y - pDepth;
                if (y < 0) return;

                x = player.x + (int)(pStartSlope * (float)pDepth);
                if (x >= util_ref.m_gen.x_dim) x = util_ref.m_gen.x_dim - 1;

                while (GetSlope(x, y, player.x, player.y, false) <= pEndSlope)
                {
                    //paint(x, y, Color.white);
                    if (GetVisDistance(x, y, player.x, player.y) <= visrange2)
                    {
                        set_visible(x, y);
                        if (is_valid(x, y) && !transparent[x, y])
                        {
                            if (x + 1 < util_ref.m_gen.x_dim && transparent[x + 1, y])
                            {
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, player.x, player.y, false));
                            }
                        }
                        else
                        {
                            if (x + 1 < util_ref.m_gen.x_dim && !transparent[x + 1, y])
                            {
                                pStartSlope = -GetSlope(x + 0.5, y - 0.5, player.x, player.y, false);
                            }
                        }
                    }
                    x--;
                }
                x++;
                break;

            case 4:
                
                x = player.x + pDepth;
                if (x >= util_ref.m_gen.x_dim) return;

                y = player.y - (int)(pStartSlope * (float)pDepth);
                if (y < 0) y = 0;

                while (GetSlope(x, y, player.x, player.y, true) <= pEndSlope)
                {
                    //paint(x, y, Color.white);
                    if (GetVisDistance(x, y, player.x, player.y) <= visrange2)
                    {
                        set_visible(x, y);
                        if (is_valid(x, y) && !transparent[x, y])
                        {
                            if (y - 1 >= 0 && transparent[x, y - 1])
                            {
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, player.x, player.y, true));
                            }
                        }
                        else
                        {
                            if (y - 1 >= 0 && !transparent[x, y - 1])
                            {
                                pStartSlope = -GetSlope(x + 0.5, y - 0.5, player.x, player.y, true);
                            }
                        }
                    }
                    y++;
                }
                y--;
                break;
            //Problem case
            case 3:
                
                x = player.x + pDepth;
                if (x >= util_ref.m_gen.x_dim) return;

                y = player.y + (int)(pStartSlope * (float)pDepth);
                if (y >= util_ref.m_gen.y_dim) y = util_ref.m_gen.y_dim - 1;

                while (GetSlope(x, y, player.x, player.y, false) >= pEndSlope)
                {
                    //paint(x, y, Color.red);
                    if (GetVisDistance(x, y, player.x, player.y) <= visrange2)
                    {
                        //paint(x, y, Color.green);
                        set_visible(x, y);
                        //We're sitting on a wall
                        if (is_valid(x, y) && !transparent[x, y])
                        {
                            //paint(x, y, Color.yellow);
                            //Disabled as one-tile blocks were being treated as transparent. No negative effects so far
                            /*
                            if (is_valid(x, y + 1) && transparent[x, y + 1])
                            {
                                paint(x, y, Color.blue);
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y + 0.5, player.x, player.y, true));
                            }
                            */
                        }
                        else
                        {
                            //paint(x, y, Color.magenta);
                            if (y + 1 < util_ref.m_gen.y_dim && !transparent[x, y + 1])
                            {
                                //paint(x, y, Color.cyan);
                                pStartSlope = GetSlope(x + 0.5, y + 0.5, player.x, player.y, true);
                            }
                        }
                    }
                    y--;
                }
                y++;
                break;

            case 2:
                
                y = player.y + pDepth;
                if (y >= util_ref.m_gen.y_dim) return;

                x = player.x + (int)(pStartSlope * (float)pDepth);
                if (x >= util_ref.m_gen.x_dim) x = util_ref.m_gen.x_dim - 1;

                while (GetSlope(x, y, player.x, player.y, false) >= pEndSlope)
                {
                    //paint(x, y, Color.white);
                    if (GetVisDistance(x, y, player.x, player.y) <= visrange2)
                    {
                        set_visible(x, y);
                        if (is_valid(x,y) && !transparent[x, y])
                        {
                            if (x + 1 < util_ref.m_gen.y_dim && transparent[x + 1, y])
                            {
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, player.x, player.y, false));
                            }
                        }
                        else
                        {
                            if (x + 1 < util_ref.m_gen.y_dim && !transparent[x + 1, y])
                            {
                                pStartSlope = GetSlope(x + 0.5, y + 0.5, player.x, player.y, false);
                            }
                        }
                    }
                    x--;
                }
                x++;
                break;

            case 1:
                
                y = player.y + pDepth;
                if (y >= util_ref.m_gen.y_dim) return;

                x = player.x - (int)(pStartSlope * (float)pDepth);
                if (x < 0) x = 0;

                while (GetSlope(x, y, player.x, player.y, false) <= pEndSlope)
                {
                    //paint(x, y, Color.white);
                    if (GetVisDistance(x, y, player.x, player.y) <= visrange2)
                    {
                        set_visible(x, y);
                        if (is_valid(x, y) && !transparent[x, y])
                        {
                            if (x - 1 >= 0 && transparent[x - 1, y])
                            {
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x - 0.5, y - 0.5, player.x, player.y, false));
                            }
                        }
                        else
                        {
                            if (x - 1 >= 0 && !transparent[x - 1, y])
                            {
                                pStartSlope = -GetSlope(x - 0.5, y + 0.5, player.x, player.y, false);
                            }
                        }
                    }
                    x++;
                }
                x--;
                break;

            case 8:
                
                x = player.x - pDepth;
                if (x < 0) return;

                y = player.y + (int)(pStartSlope * (float)pDepth);
                if (y >= util_ref.m_gen.y_dim) y = util_ref.m_gen.y_dim - 1;

                while (GetSlope(x, y, player.x, player.y, true) <= pEndSlope)
                {
                    //paint(x, y, Color.red);
                    if (GetVisDistance(x, y, player.x, player.y) <= visrange2)
                    {
                        //paint(x, y, Color.green);
                        set_visible(x, y);
                        if (is_valid(x, y) && !transparent[x, y])
                        {
                            if (y + 1 < util_ref.m_gen.y_dim && transparent[x, y + 1])
                            {
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y + 0.5, player.x, player.y, true));
                            }
                        }
                        else
                        {
                            if (y + 1 < util_ref.m_gen.y_dim && !transparent[x, y + 1])
                            {
                                pStartSlope = -GetSlope(x - 0.5, y + 0.5, player.x, player.y, true);
                            }
                        }
                    }
                    y--;
                }
                y++;
                break;

            case 7: //wnw
                
                x = player.x - pDepth;
                if (x < 0) return;

                y = player.y - (int)(pStartSlope * (float)pDepth);
                if (y < 0) y = 0;

                while (GetSlope(x, y, player.x, player.y, true) >= pEndSlope)
                {
                    //paint(x, y, Color.white);
                    if (GetVisDistance(x, y, player.x, player.y) <= visrange2)
                    {
                        set_visible(x, y);
                        if (is_valid(x, y) && !transparent[x, y])
                        {
                            if (y - 1 >= 0 && transparent[x, y - 1])
                            {
                                ScanOctant(pDepth + 1, pOctant, pStartSlope, GetSlope(x + 0.5, y - 0.5, player.x, player.y, true));
                            }

                        }
                        else
                        {
                            if (y - 1 >= 0 && !transparent[x, y - 1])
                            {
                                pStartSlope = GetSlope(x - 0.5, y - 0.5, player.x, player.y, true);
                            }
                        }
                    }
                    y++;
                }
                y--;
                break;
        }


        if (x < 0)
            x = 0;
        else if (x >= util_ref.m_gen.x_dim)
            x = util_ref.m_gen.x_dim - 1;

        if (y < 0)
            y = 0;
        else if (y >= util_ref.m_gen.y_dim)
            y = util_ref.m_gen.y_dim - 1;

        if (pDepth < visual_range & transparent[x, y])
            ScanOctant(pDepth + 1, pOctant, pStartSlope, pEndSlope);

    }

    /// <summary>
    /// Get the gradient of the slope formed by the two points
    /// </summary>
    /// <param name="pX1"></param>
    /// <param name="pY1"></param>
    /// <param name="pX2"></param>
    /// <param name="pY2"></param>
    /// <param name="pInvert">Invert slope</param>
    /// <returns></returns>
    private double GetSlope(double pX1, double pY1, double pX2, double pY2, bool pInvert)
    {
        if (pInvert)
            return (pY1 - pY2) / (pX1 - pX2);
        else
            return (pX1 - pX2) / (pY1 - pY2);
    }


    /// <summary>
    /// Calculate the distance between the two points
    /// </summary>
    /// <param name="pX1"></param>
    /// <param name="pY1"></param>
    /// <param name="pX2"></param>
    /// <param name="pY2"></param>
    /// <returns>Distance</returns>
    private int GetVisDistance(int pX1, int pY1, int pX2, int pY2)
    {
        return ((pX1 - pX2) * (pX1 - pX2)) + ((pY1 - pY2) * (pY1 - pY2));
    }

    #endregion

}