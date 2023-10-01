using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;

namespace HZDZTOUHOU.NPCs.Bosses
{
    [AutoloadBossHead]
    public class reimu : ModNPC
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Reimu");
            Main.npcFrameCount[NPC.type] = 16;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            NPCID.Sets.TownCritter[NPC.type] = false;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1; // 设置ai类型
            NPC.lifeMax = 5500; // 最大生命值
            NPC.defense = 0; // 防御力
            NPC.damage = 25; // 攻击力
            
            NPC.knockBackResist = 0f; // 击退抵抗
            NPC.width = 64; // 宽度
            NPC.height = 80; // 高度
            NPC.value = Item.buyPrice(0, 5, 0, 0); // 击杀后掉落金币数
            NPC.HitSound = SoundID.NPCHit7; // 受伤音效
            NPC.DeathSound = SoundID.NPCDeath2; // 死亡音效
            NPC.boss = true; // 表示此NPC为Boss
            NPC.noGravity = true; // Boss不受重力影响
            NPC.noTileCollide = true; // 穿过砖块
            NPC.lavaImmune = true; // 免疫岩浆伤害
            NPC.netAlways = true;
            NPC.scale = 1f;
            NPC.npcSlots = 6f;

            //难度微调数值
            if (Main.expertMode == true)
            {
                NPC.lifeMax = 10800 / 2;
            }
            if (Main.masterMode == true)
            {
                NPC.lifeMax = 12000 / 3;
            }
        }

        public override void AI()
        {
            //设置状态和参数
            int state = 1;
            if(Main.expertMode == true)
            {
                if (NPC.life >= (NPC.lifeMax * 2 / 3))
                {
                    state = 1;
                    NPC.defense = 5;
                }
                if (NPC.life >= (NPC.lifeMax / 3) && NPC.life <= (NPC.lifeMax * 2 / 3))
                {
                    state = 2;
                    NPC.defense = 9999;
                    //创建环形粒子效果
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        // 添加粒子效果
                        for(int i = 0; i < 360; i++)
                        {
                            Vector2 position = NPC.Center;
                            Vector2 direction;
                            var PI = MathF.PI;
                            var size = 1000; //设置半径
                            direction.X = MathF.Cos(i / 180f * PI);
                            direction.Y = MathF.Sin(i / 180f * PI);
                            direction.Normalize();
                            position.X += direction.X * size;
                            position.Y += direction.Y * size;
                            int dust = Dust.NewDust(position, 4, 4, DustID.FlameBurst, 0f, 0f, 0, default(Color), 2f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].noLight = false;
                            Main.dust[dust].velocity *= 0f;
                        }
                        
                    }
                }
                if (NPC.life <= (NPC.lifeMax / 3))
                {
                    state = 3;
                    NPC.defense = 5;
                }
            }
            else
            {
                if (NPC.life >= (NPC.lifeMax / 2))
                {
                    state = 1;
                    NPC.defense = 5;
                }
                if (NPC.life <= (NPC.lifeMax / 2))
                {
                    state = 3;
                    NPC.defense = 5;
                }
            }

            //根据难度设置值
            int dmg = NPC.damage;
            if(Main.masterMode == true)
            {
                dmg /= 4;
            }
            else
            {
                if (Main.expertMode == true)
                {
                    dmg = dmg * 2 / 5; //除以2.5
                }
                else
                {
                    dmg = dmg * 2 / 3; //除以1.5
                }

            }

            //P1 每1秒发射n3/e4/m5连发的射弹
            int count;
            if (Main.masterMode == true)
            {
                count = 14;
            }
            else
            {
                if (Main.expertMode == true)
                {
                    count = 11;
                }
                else
                {
                    count = 8 ;
                }

            }
            if (NPC.ai[0] % 60 <= count && NPC.ai[0] % 3 == 0 && state == 1 )
            {
                
                //Projectile.NewProjectile(NPC.Center.X, NPC.Center.Y, 0f, 0f, Mod.Find<ModProjectile>("reimu1").Type, NPC.damage / 3, 1f, Main.myPlayer);
                //Projectile.NewProjectile(NPC.Center, Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 10f, Mod.Find<ModProjectile>("reimu1").Type, 50, 10f); // 发射射弹
                NPC.TargetClosest();
                if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    var source = NPC.GetSource_FromAI();
                    Vector2 position = NPC.Center;
                    Vector2 targetPosition = Main.player[NPC.target].Center;
                    Random rd1 = new Random();
                    targetPosition.X += rd1.Next(-50, 50);
                    targetPosition.Y += rd1.Next(-50, 50);
                    Vector2 direction = targetPosition - position;
                    direction.Normalize();
                    float speed1 = 8f;
                    int type = Mod.Find<ModProjectile>("reimu1").Type;
                    //int damage = NPC.damage; //If the projectile is hostile, the damage passed into NewProjectile will be applied doubled, and quadrupled if expert mode, so keep that in mind when balancing projectiles if you scale it off NPC.damage (which also increases for expert/master)
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(source, position, direction * speed1, type, dmg, 0f, Main.myPlayer);
                    }
                }
                
            }
            
            //P1 n每2.5秒发射一圈环形弹幕(20)e每2秒发射一圈环形弹幕(24)m每1.5秒发射一圈环形弹幕(30) 角度随机
            if (Main.masterMode == true)
            {
                Random rd4 = new Random();
                NPC.ai[1] = rd4.Next(0, 12);
                if (NPC.ai[0] % 90 == 0 && state == 1)//大师
                {
                    NPC.ai[1] = 0f;
                    for (int i = 0; i < 30; i += 1)
                    {
                        NPC.ai[1] += 12f;

                        var source = NPC.GetSource_FromAI();
                        Vector2 position = NPC.Center;
                        Vector2 direction;
                        //设置directiom
                        var PI = MathF.PI;
                        direction.X = MathF.Cos(NPC.ai[1] / 180f * PI);
                        direction.Y = MathF.Sin(NPC.ai[1] / 180f * PI);
                        direction.Normalize();
                        float speed2 = 7f;
                        int type = Mod.Find<ModProjectile>("reimu1").Type;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                        }
                    }
                }
            }
            else
            {
                if (Main.expertMode == true)
                {
                    if (NPC.ai[0] % 120 == 0 && state == 1)//专家
                    {
                        Random rd4 = new Random();
                        NPC.ai[1] = rd4.Next(0, 15);
                        for (int i = 0; i < 24; i += 1)
                        {
                            NPC.ai[1] += 15f;

                            var source = NPC.GetSource_FromAI();
                            Vector2 position = NPC.Center;
                            Vector2 direction;
                            //设置directiom
                            var PI = MathF.PI;
                            direction.X = MathF.Cos(NPC.ai[1] / 180f * PI);
                            direction.Y = MathF.Sin(NPC.ai[1] / 180f * PI);
                            direction.Normalize();
                            float speed2 = 7f;
                            int type = Mod.Find<ModProjectile>("reimu1").Type;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                            }
                        }
                    }
                }
                else
                {
                    if (NPC.ai[0] % 150 == 0 && state == 1)//普通
                    {
                        Random rd4 = new Random();
                        NPC.ai[1] = rd4.Next(0, 18);
                        for (int i = 0; i < 20; i += 1)
                        {
                            NPC.ai[1] += 18f;

                            var source = NPC.GetSource_FromAI();
                            Vector2 position = NPC.Center;
                            Vector2 direction;
                            //设置directiom
                            var PI = MathF.PI;
                            direction.X = MathF.Cos(NPC.ai[1] / 180f * PI);
                            direction.Y = MathF.Sin(NPC.ai[1] / 180f * PI);
                            direction.Normalize();
                            float speed2 = 7f;
                            int type = Mod.Find<ModProjectile>("reimu1").Type;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                            }
                        }
                    }
                }

            }


            //P3 n每2s在左右各发射随机角度的环形弹幕(24) e每1.5s在左右各发射随机角度的环形弹幕(30) m每1.5s在左右各发射随机角度的环形弹幕(30+36)
            if (Main.masterMode == true)
            {
                if (NPC.ai[0] % 90 == 0 && state == 3)
                {
                    Random rd3 = new Random();
                    NPC.ai[1] = rd3.Next(0, 10);
                    for (int i = 0; i < 36; i += 1)
                    {
                        NPC.ai[1] += 10f;

                        var source = NPC.GetSource_FromAI();
                        Vector2 position = NPC.Center;
                        position.X += 80f;
                        Vector2 direction;
                        //设置directiom
                        var PI = MathF.PI;
                        direction.X = MathF.Cos(NPC.ai[1] / 180f * PI);
                        direction.Y = MathF.Sin(NPC.ai[1] / 180f * PI);
                        direction.Normalize();
                        float speed2 = 6f;
                        int type = Mod.Find<ModProjectile>("reimu1").Type;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                        }
                    }
                    NPC.ai[1] = rd3.Next(0, 12);
                    for (int i = 0; i < 30; i += 1)
                    {
                        NPC.ai[1] += 12f;

                        var source = NPC.GetSource_FromAI();
                        Vector2 position = NPC.Center;
                        position.X -= 80f;
                        Vector2 direction;
                        //设置directiom
                        var PI = MathF.PI;
                        direction.X = MathF.Cos(NPC.ai[1] / 180f * PI);
                        direction.Y = MathF.Sin(NPC.ai[1] / 180f * PI);
                        direction.Normalize();
                        float speed2 = 6f;
                        int type = Mod.Find<ModProjectile>("reimu1").Type;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                        }
                    }
                }
            }
            else
            {
                if (Main.expertMode == true)
                {
                    if (NPC.ai[0] % 90 == 0 && state == 3)
                    {
                        Random rd3 = new Random();
                        NPC.ai[1] = rd3.Next(0, 12);
                        for (int i = 0; i < 30; i += 1)
                        {
                            NPC.ai[1] += 12f;

                            var source = NPC.GetSource_FromAI();
                            Vector2 position = NPC.Center;
                            position.X += 80f;
                            Vector2 direction;
                            //设置directiom
                            var PI = MathF.PI;
                            direction.X = MathF.Cos(NPC.ai[1] / 180f * PI);
                            direction.Y = MathF.Sin(NPC.ai[1] / 180f * PI);
                            direction.Normalize();
                            float speed2 = 6f;
                            int type = Mod.Find<ModProjectile>("reimu1").Type;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                            }
                        }
                        NPC.ai[1] = rd3.Next(0, 12);
                        for (int i = 0; i < 30; i += 1)
                        {
                            NPC.ai[1] += 12f;

                            var source = NPC.GetSource_FromAI();
                            Vector2 position = NPC.Center;
                            position.X -= 80f;
                            Vector2 direction;
                            //设置directiom
                            var PI = MathF.PI;
                            direction.X = MathF.Cos(NPC.ai[1] / 180f * PI);
                            direction.Y = MathF.Sin(NPC.ai[1] / 180f * PI);
                            direction.Normalize();
                            float speed2 = 6f;
                            int type = Mod.Find<ModProjectile>("reimu1").Type;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                            }
                        }
                    }
                }
                else
                {
                    if (NPC.ai[0] % 120 == 0 && state == 3)
                    {
                        Random rd3 = new Random();
                        NPC.ai[1] = rd3.Next(0, 15);
                        for (int i = 0; i < 24; i += 1)
                        {
                            NPC.ai[1] += 15f;

                            var source = NPC.GetSource_FromAI();
                            Vector2 position = NPC.Center;
                            position.X += 80f;
                            Vector2 direction;
                            //设置directiom
                            var PI = MathF.PI;
                            direction.X = MathF.Cos(NPC.ai[1] / 180f * PI);
                            direction.Y = MathF.Sin(NPC.ai[1] / 180f * PI);
                            direction.Normalize();
                            float speed2 = 6f;
                            int type = Mod.Find<ModProjectile>("reimu1").Type;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                            }
                        }
                        NPC.ai[1] = rd3.Next(0, 15);
                        for (int i = 0; i < 24; i += 1)
                        {
                            NPC.ai[1] += 15f;

                            var source = NPC.GetSource_FromAI();
                            Vector2 position = NPC.Center;
                            position.X -= 80f;
                            Vector2 direction;
                            //设置directiom
                            var PI = MathF.PI;
                            direction.X = MathF.Cos(NPC.ai[1] / 180f * PI);
                            direction.Y = MathF.Sin(NPC.ai[1] / 180f * PI);
                            direction.Normalize();
                            float speed2 = 6f;
                            int type = Mod.Find<ModProjectile>("reimu1").Type;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                            }
                        }
                    }
                }

            }


            //P2 连续发射旋转弹幕e4m5
            if (Main.masterMode == true)
            {
                if (NPC.ai[0] % 6 == 1 && state == 2)
                {
                    NPC.ai[1] = NPC.ai[0] % 720 / 2f;

                    for (int i = 0; i < 5; i += 1)
                    {
                        NPC.ai[1] += 72f;

                        var source = NPC.GetSource_FromAI();
                        Vector2 position = NPC.Center;
                        Vector2 direction;
                        //设置directiom
                        var PI = MathF.PI;
                        direction.X = MathF.Cos(NPC.ai[1] / 180f * PI);
                        direction.Y = MathF.Sin(NPC.ai[1] / 180f * PI);
                        direction.Normalize();
                        float speed2 = 6f;
                        int type = Mod.Find<ModProjectile>("reimu1").Type;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                        }
                    }
                }
            }
            else
            {
                if (Main.expertMode == true)
                {
                    if (NPC.ai[0] % 8 == 1 && state == 2)
                    {
                        NPC.ai[1] = NPC.ai[0] % 720 / 2f;

                        for (int i = 0; i < 4; i += 1)
                        {
                            NPC.ai[1] += 90f;

                            var source = NPC.GetSource_FromAI();
                            Vector2 position = NPC.Center;
                            Vector2 direction;
                            //设置directiom
                            var PI = MathF.PI;
                            direction.X = MathF.Cos(NPC.ai[1] / 180f * PI);
                            direction.Y = MathF.Sin(NPC.ai[1] / 180f * PI);
                            direction.Normalize();
                            float speed2 = 6f;
                            int type = Mod.Find<ModProjectile>("reimu1").Type;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                            }
                        }
                    }
                }
            }

            //P2 e每2.5秒发射一圈环形弹幕(15)m每2秒发射一圈环形弹幕(20) 角度随机
            if (Main.masterMode == true)
            {
                Random rd4 = new Random();
                NPC.ai[1] = rd4.Next(0, 18);
                if (NPC.ai[0] % 120 == 0 && state == 2)//大师
                {
                    NPC.ai[1] = 0f;
                    for (int i = 0; i < 20; i += 1)
                    {
                        NPC.ai[1] += 18f;

                        var source = NPC.GetSource_FromAI();
                        Vector2 position = NPC.Center;
                        Vector2 direction;
                        //设置directiom
                        var PI = MathF.PI;
                        direction.X = MathF.Cos(NPC.ai[1] / 180f * PI);
                        direction.Y = MathF.Sin(NPC.ai[1] / 180f * PI);
                        direction.Normalize();
                        float speed2 = 6f;
                        int type = Mod.Find<ModProjectile>("reimu1").Type;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                        }
                    }
                }
            }
            else
            {
                if (Main.expertMode == true)
                {
                    if (NPC.ai[0] % 150 == 0 && state == 2)//专家
                    {
                        Random rd4 = new Random();
                        NPC.ai[1] = rd4.Next(0, 24);
                        for (int i = 0; i < 15; i += 1)
                        {
                            NPC.ai[1] += 24f;

                            var source = NPC.GetSource_FromAI();
                            Vector2 position = NPC.Center;
                            Vector2 direction;
                            //设置directiom
                            var PI = MathF.PI;
                            direction.X = MathF.Cos(NPC.ai[1] / 180f * PI);
                            direction.Y = MathF.Sin(NPC.ai[1] / 180f * PI);
                            direction.Normalize();
                            float speed2 = 6f;
                            int type = Mod.Find<ModProjectile>("reimu1").Type;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                            }
                        }
                    }
                }

            }

            //P1 P3 向玩家移动 距离150格停下
            if (state != 2)
            {
                NPC.TargetClosest();
                Player target = Main.player[NPC.target];
                Vector2 targetPosition = target.Center;
                if(Math.Abs(targetPosition.X - NPC.Center.X) > 150f ||
                    Math.Abs(targetPosition.Y - NPC.Center.Y) > 150f)
                {
                    if (Math.Abs(targetPosition.X - NPC.Center.X) <= 300f &&
                    Math.Abs(targetPosition.Y - NPC.Center.Y) <= 300f)
                    {
                        NPC.velocity.X = 0f; 
                        NPC.velocity.Y = 0f;
                    }
                    else
                    {
                        targetPosition.Y -= 300;
                        //Random rd2 = new Random();
                        //targetPosition.X += rd2.Next(-50, 50);
                        //targetPosition.Y += rd2.Next(-50, 50);
                        float speed3 = 4f;
                        Vector2 ToPlayer = NPC.DirectionTo(targetPosition) * speed3;
                        NPC.velocity = ToPlayer;
                    }
                    
                    
                }
            }
            //P2 站桩,自己掉血,惩罚
            if(state == 2)
            {
                NPC.velocity.X = 0f;
                NPC.velocity.Y = 0f;
                if (NPC.ai[0] % 15 == 1 && Playeramount()>=1)
                {
                    NPC.life -= NPC.lifeMax / 3 / 80; //20s死
                }

                // 惩罚圈外玩家
                List<Player> players = new List<Player>();
                for (int i = 0; i < Main.player.Length; i++)
                {
                    Player player = Main.player[i];
                    if (player.active)
                    {
                        players.Add(player);
                    }
                }
                float radius = 1000f; // 检测半径
                foreach (Player player in players)
                {
                    if (Vector2.Distance(NPC.Center, player.Center) >= radius && player.dead != true)
                    {
                        //惩罚
                        if (Main.masterMode == true )//大师
                        {
                            Random rd4 = new Random();
                            int count1 = 12;
                            
                            if (NPC.ai[0] % 300 == 0 && state == 2)
                            {
                                NPC.ai[1] = rd4.Next(0, 360 / count1);
                                for (int i = 0; i < count1; i += 1)
                                {
                                    NPC.ai[1] += 360 / count1;
                                    float distanse = 250f;
                                    var source = NPC.GetSource_FromAI();
                                    Vector2 position = player.Center;
                                    var PI = MathF.PI;
                                    position.X += MathF.Cos(NPC.ai[1] / 180f * PI) * distanse;
                                    position.Y += MathF.Sin(NPC.ai[1] / 180f * PI) * distanse;
                                    Vector2 direction = player.Center - position ;
                                    direction.Normalize();
                                    float speed2 = 1.6f;
                                    int type = Mod.Find<ModProjectile>("reimu1").Type;
                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Main.expertMode == true) // 专家
                            {
                                Random rd4 = new Random();
                                int count1 = 10;

                                if (NPC.ai[0] % 360 == 0 && state == 2)
                                {
                                    NPC.ai[1] = rd4.Next(0, 360 / count1);
                                    for (int i = 0; i < count1; i += 1)
                                    {
                                        NPC.ai[1] += 360 / count1;
                                        float distanse = 250f;
                                        var source = NPC.GetSource_FromAI();
                                        Vector2 position = player.Center;
                                        var PI = MathF.PI;
                                        position.X += MathF.Cos(NPC.ai[1] / 180f * PI) * distanse;
                                        position.Y += MathF.Sin(NPC.ai[1] / 180f * PI) * distanse;
                                        Vector2 direction = player.Center - position;
                                        direction.Normalize();
                                        float speed2 = 1.2f;
                                        int type = Mod.Find<ModProjectile>("reimu1").Type;
                                        if (Main.netMode != NetmodeID.MultiplayerClient)
                                        {
                                            Projectile.NewProjectile(source, position, direction * speed2, type, dmg, 0f, Main.myPlayer);
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }


            NPC.ai[0]++;
        }

        public char getFrameStatus(int framepertick)
        {
            if ((NPC.frameCounter < framepertick * 4 - 1) && (NPC.frameCounter >= framepertick * 0))
            {
                return '1';//飞行-不关键
            }
            if(NPC.frameCounter == framepertick * 4 -1)
            {
                return '2';//飞行-关键
            }
            if ((NPC.frameCounter < framepertick * 8 - 1) && (NPC.frameCounter >= framepertick * 4))
            {
                return '3';//向右-不关键
            }
            if (NPC.frameCounter == framepertick * 8 - 1)
            {
                return '4';//向右-关键
            }
            if ((NPC.frameCounter < framepertick * 12 - 1) && (NPC.frameCounter >= framepertick * 8))
            {
                return '5';//向左-不关键
            }
            if (NPC.frameCounter == framepertick * 12 - 1)
            {
                return '6';//向左-关键
            }
            if ((NPC.frameCounter < framepertick * 16 - 1) && (NPC.frameCounter >= framepertick * 12))
            {
                return '7';//攻击-不关键
            }
            if (NPC.frameCounter == framepertick * 16 - 1)
            {
                return '8';//攻击-关键
            }
            return '9';
        }

        
        public override void FindFrame(int frameHeight)
        {
            //动画
            var framepertick = 5;
            NPC.frame.Y = frameHeight * ((int)NPC.frameCounter / framepertick);

            switch (getFrameStatus(framepertick))
            {
                case '1':
                    if (NPC.velocity.X > 0) { NPC.frameCounter = framepertick * 4; break; }
                    if (NPC.velocity.X < 0) { NPC.frameCounter = framepertick * 8; break; }
                    NPC.frameCounter++;
                    break;
                case '3':
                case '5':
                case '7':
                    NPC.frameCounter++; 
                    break;
                case '2':
                    if (NPC.life >= (NPC.lifeMax / 3) && NPC.life <= (NPC.lifeMax * 2 / 3)) { NPC.frameCounter = framepertick * 12; break; }
                    if (NPC.velocity.X > 0) NPC.frameCounter = framepertick * 4 ;
                    if (NPC.velocity.X < 0) NPC.frameCounter = framepertick * 8 ;
                    if (NPC.velocity.X == 0) NPC.frameCounter = framepertick * 0 ;
                    break;
                case '4':
                    if (NPC.life >= (NPC.lifeMax / 3) && NPC.life <= (NPC.lifeMax * 2 / 3)) { NPC.frameCounter = framepertick * 12; break; }
                    if (NPC.velocity.X > 0) NPC.frameCounter = framepertick * 7 ;
                    if (NPC.velocity.X < 0) NPC.frameCounter = framepertick * 8 ;
                    if (NPC.velocity.X == 0) NPC.frameCounter = framepertick * 0 ;
                    break;
                case '6':
                    if (NPC.life >= (NPC.lifeMax / 3) && NPC.life <= (NPC.lifeMax * 2 / 3)) { NPC.frameCounter = framepertick * 12; break; }
                    if (NPC.velocity.X > 0) NPC.frameCounter = framepertick * 4 ;
                    if (NPC.velocity.X < 0) NPC.frameCounter = framepertick * 11 ;
                    if (NPC.velocity.X == 0) NPC.frameCounter = framepertick * 0 ;
                    break;
                case '8':
                    if (NPC.life >= (NPC.lifeMax / 3) && NPC.life <= (NPC.lifeMax * 2 / 3)) { NPC.frameCounter = framepertick * 0 ; break; }
                    if (NPC.velocity.X > 0) NPC.frameCounter = framepertick * 4;
                    if (NPC.velocity.X < 0) NPC.frameCounter = framepertick * 8;
                    if (NPC.velocity.X == 0) NPC.frameCounter = framepertick * 0;
                    break;
                default:
                    NPC.frameCounter = 0;
                    break;
            }

            
            
            
            
        }
            
        
        public int Playeramount()
        {
            // 获取所有活着的玩家列表
            List<Player> players = new List<Player>();
            for (int i = 0; i < Main.player.Length; i++)
            {
                Player player = Main.player[i];
                if (player.active)
                {
                    players.Add(player);
                }
            }
            // 遍历所有玩家并检查是否在boss附近
            float radius = 1000f; // 检测半径
            int playerCount = 0;

            foreach (Player player in players)
            {
                if (Vector2.Distance(NPC.Center,player.Center) <= radius && player.dead!=true)
                {
                    playerCount++;
                }
            }
            return playerCount;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            // 添加淡出特效
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, 0f, 100, default(Color), 10f);
                Main.dust[dust].velocity *= 1.4f;
                Main.dust[dust].noGravity = true;
            }

            // 移除NPC
            NPC.life = 0;
            NPC.active = false;

            // 显示消息
            if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.SendData(MessageID.WorldData); // 向所有客户端发送数据以更新NPC
            }
            else
            {
                Main.NewText("Reimu has run away!", Color.MediumPurple);
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

    }
}

