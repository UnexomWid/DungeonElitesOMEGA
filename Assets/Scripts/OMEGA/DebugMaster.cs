using UnityEngine;
using UnityEngine.Profiling;

namespace OMEGA
{
    public class DebugMaster : MonoBehaviour
    {
        public const float FREECAM_SPEED = 50f;

        static bool alive = false;

        public static bool freecam;

        static GUIStyle infoStyle;
        static GUIStyle freecamStyle;

        static bool showInfo;

        void Awake()
        {
            if (alive)
            {
                Destroy(gameObject);
                return;
            }

            alive = true;

            infoStyle = new GUIStyle();
            infoStyle.fontSize = 16;
            infoStyle.normal.textColor = Color.white;

            freecamStyle = new GUIStyle();
            freecamStyle.fontSize = 24;
            freecamStyle.normal.textColor = Color.white;

            showInfo = false;
            freecam = false;

            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            try
            {
                HandleRevealMap();
                HandleInfo();
                HandleFreecam();
                HandleMidas();
                HandleClaimArtifact();
                HandleInstakill();
            }
            catch (System.Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        void HandleRevealMap()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                room[] rooms = FindObjectsOfType<room>();

                if (rooms == null)
                {
                    return;
                }

                foreach (room r in rooms)
                {
                    r.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                    r.discovered = true;
                }
            }
        }

        void HandleInfo()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                showInfo = !showInfo;
            }
        }

        void HandleFreecam()
        {
            if (Input.GetKeyDown(KeyCode.F4))
            {
                freecam = !freecam;
            }

            if (freecam && CameraFollow.instance != null)
            {
                var cam = CameraFollow.instance.GetComponent<Transform>();

                float x = 0f;
                float y = 0f;

                if (Input.GetKey(KeyCode.A))
                {
                    x -= 1f;
                }
                if (Input.GetKey(KeyCode.D))
                {
                    x += 1f;
                }
                if (Input.GetKey(KeyCode.W))
                {
                    y += 1f;
                }
                if (Input.GetKey(KeyCode.S))
                {
                    y -= 1f;
                }

                float speed = FREECAM_SPEED;

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    speed *= 2;
                }

                Vector2 vel = (new Vector2(x, y)).normalized * speed * Time.deltaTime;

                cam.position = new Vector3(cam.position.x + vel.x, cam.position.y + vel.y, cam.position.z);

                if (Input.GetKeyDown(KeyCode.Space) && Data.players != null)
                {
                    foreach (var player in Data.players)
                    {
                        var tform = player.GetComponent<Transform>();

                        tform.position = new Vector3(cam.position.x, cam.position.y, tform.position.z);
                    }
                }
            }
        }

        void HandleMidas()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                var inventory = FindObjectOfType<InventorySpawn>();

                if (inventory)
                {
                    inventory.coins = 1000000;
                }

                if (Data.players.Count > 0)
                {
                    Data.players[0].health = 9999999f;

                    var wiz = Data.players[0].wizard;

                    if (wiz != null)
                    {
                        wiz.staffDamage = 9999999f;
                    }
                }
            }
        }

        void HandleClaimArtifact()
        {
            if (Input.GetKeyDown(KeyCode.F6))
            {
                if (Data.artifact != null)
                {
                    Data.artifact.GetComponent<ArtifactAvaritia>().Claim(Data.players[0]);
                }
            }
        }

        void HandleInstakill()
        {
            if (Input.GetKeyDown(KeyCode.F7))
            {
                var players = FindObjectsOfType<player>();

                foreach (var player in players)
                {
                    var p = player.GetComponent<player>();

                    if (p != null && (p.bot || p.boss))
                    {
                        p.DecreaseHp(99999999f, Vector2.zero, Data.players[0]);
                    }
                }
            }
        }

        void OnGUI()
        {
            if (CameraFollow.instance == null)
            {
                return;
            }

            if (showInfo)
            {
                Transform tform = CameraFollow.instance.GetComponent<Transform>();

                string reserved = (Profiler.GetTotalReservedMemoryLong() / (1024 * 1024)).ToString();
                string allocated = (Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024)).ToString().PadLeft(reserved.Length, ' ');

                string info = string.Format("x: {0}\ny: {1}\n---\nMem {2}/{3} MB\nF2 - Spy satellite\nF3 - Show this\nF4 - Freecam\nF5 - Midas\nF6 - Claim artifact\nF7 - Instakill", tform.position.x, tform.position.y, allocated, reserved);

                GUI.Label(new Rect(10f, 10f, (float)(Screen.width / 9), (float)(Screen.width / 9)), info, infoStyle);
            }

            if (freecam)
            {
                GUI.Label(new Rect(Screen.width / 2 - 55f, 10f, (float)(Screen.width / 8), (float)(Screen.width / 12)), "-- FREECAM --", freecamStyle);
            }
        }
    }
}