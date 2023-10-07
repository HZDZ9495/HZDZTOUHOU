using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using Mono.Cecil;

namespace HZDZTOUHOU.NPCs.Bosses
{
    [AutoloadBossHead]
    public class normalReimu : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 16;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            NPCID.Sets.TownCritter[NPC.type] = false;
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1; // 设置ai类型
            NPC.lifeMax = 25000; // 最大生命值
            NPC.defense = 0; // 防御力
            NPC.damage = 40; // 攻击力

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
            

            //根据难度设置伤害值
            int dmg = NPC.damage;
            if (Main.masterMode == true)
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

            //设置阶段
            if(NPC.life>=(NPC.lifeMax * 0.8f))
            {
                state = 1;
            }

            //P1 每1秒发射n4/e5/m6连发的射弹(预判）
            int count, chance;
            if (Main.masterMode == true)
            {
                count = 17;
                chance = 50;
            }
            else
            {
                if (Main.expertMode == true)
                {
                    count = 14;
                    chance = 35;
                }
                else
                {
                    count = 11;
                    chance = 20;
                }

            }
            
            if (NPC.ai[0] % 60 <= count && NPC.ai[0] % 3 == 0 && state == 1)
            {

                NPC.TargetClosest();
                if (NPC.HasValidTarget && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    var source = NPC.GetSource_FromAI();
                    Vector2 position = NPC.Center;
                    Vector2 targetPosition = Main.player[NPC.target].Center;
                    //Vector2 targetVelocity = Main.player[NPC.target].velocity;
                    //targetVelocity.Normalize();
                    //float speed2 = Math.Abs(Main.player[NPC.target].velocity.X / targetVelocity.X);
                    Random rd1 = new Random();
                    //int time1 = rd1.Next((int)speed2 * 2, (int)speed2 * 5);
                    int time1 = rd1.Next(45, 75);
                    float speed1 = 12f;
                    Vector2 direction = (targetPosition - position + Main.player[NPC.target].velocity * time1) / (speed1 * time1);
                    direction.Normalize();
                    
                    //按概率替换伤害增加的射弹
                    if(rd1.Next(1,100)<=chance)
                    {
                        int type = Mod.Find<ModProjectile>("Reimu_projectile2_LT").Type;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(source, position, direction * speed1, type, (int)(dmg * 1.5f), 0f, Main.myPlayer);
                        }
                    }
                    else
                    {
                        int type = Mod.Find<ModProjectile>("Reimu_projectile1_LT").Type;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(source, position, direction * speed1, type, dmg, 0f, Main.myPlayer);
                        }
                    }
                }

            }
            
            if (NPC.ai[0]%30==1 && state == 1)
            {
                NPC.TargetClosest();
                var source = NPC.GetSource_FromAI();
                Vector2 position = Main.player[NPC.target].Center;
                float speed1 = 0.001f;
                Random rd2 = new Random();
                int angel1 = rd2.Next(1, 360);
                var PI = MathF.PI;
                float distance = 200f;
                position.X += MathF.Cos(angel1 / 180f * PI) * distance;
                position.Y += MathF.Sin(angel1 / 180f * PI) * distance;
                Vector2 direction = Main.player[NPC.target].Center - position;
                direction.Normalize();
                int type = Mod.Find<ModProjectile>("Reimu_projectile1_DL").Type;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(source, position, direction * speed1, type, dmg, 0f, Main.myPlayer);
                }
            }

            //P1 向玩家移动 距离150格停下
            if (state == 1)
            {
                NPC.TargetClosest();
                Player target = Main.player[NPC.target];
                Vector2 targetPosition = target.Center;
                if (Math.Abs(targetPosition.X - NPC.Center.X) > 150f ||
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
                        float speed3 = 8f;
                        Vector2 ToPlayer = NPC.DirectionTo(targetPosition) * speed3;
                        NPC.velocity = ToPlayer;
                    }


                }
            }
            //P2 站桩,自己掉血,惩罚

            NPC.ai[0]++;
        }

        public char getFrameStatus(int framepertick)
        {
            if ((NPC.frameCounter < framepertick * 4 - 1) && (NPC.frameCounter >= framepertick * 0))
            {
                return '1';//飞行-不关键
            }
            if (NPC.frameCounter == framepertick * 4 - 1)
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
                    if (NPC.velocity.X > 0) NPC.frameCounter = framepertick * 4;
                    if (NPC.velocity.X < 0) NPC.frameCounter = framepertick * 8;
                    if (NPC.velocity.X == 0) NPC.frameCounter = framepertick * 0;
                    break;
                case '4':
                    if (NPC.life >= (NPC.lifeMax / 3) && NPC.life <= (NPC.lifeMax * 2 / 3)) { NPC.frameCounter = framepertick * 12; break; }
                    if (NPC.velocity.X > 0) NPC.frameCounter = framepertick * 7;
                    if (NPC.velocity.X < 0) NPC.frameCounter = framepertick * 8;
                    if (NPC.velocity.X == 0) NPC.frameCounter = framepertick * 0;
                    break;
                case '6':
                    if (NPC.life >= (NPC.lifeMax / 3) && NPC.life <= (NPC.lifeMax * 2 / 3)) { NPC.frameCounter = framepertick * 12; break; }
                    if (NPC.velocity.X > 0) NPC.frameCounter = framepertick * 4;
                    if (NPC.velocity.X < 0) NPC.frameCounter = framepertick * 11;
                    if (NPC.velocity.X == 0) NPC.frameCounter = framepertick * 0;
                    break;
                case '8':
                    if (NPC.life >= (NPC.lifeMax / 3) && NPC.life <= (NPC.lifeMax * 2 / 3)) { NPC.frameCounter = framepertick * 0; break; }
                    if (NPC.velocity.X > 0) NPC.frameCounter = framepertick * 4;
                    if (NPC.velocity.X < 0) NPC.frameCounter = framepertick * 8;
                    if (NPC.velocity.X == 0) NPC.frameCounter = framepertick * 0;
                    break;
                default:
                    NPC.frameCounter = 0;
                    break;
            }





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

