using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HZDZTOUHOU.Projectiles
{
    public class Reimu_projectile1_DL : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("reimu's projectile");
            Main.npcFrameCount[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.arrow = false;
            Projectile.width = 14;
            Projectile.height = 12;
            Projectile.aiStyle = -1; // or 1
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            Projectile.hostile = false;
            Projectile.maxPenetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;

            if (Projectile.ai[0]==60)
            {
                Projectile.velocity = Projectile.velocity * 6000;
                Projectile.hostile = true;
            }
            Projectile.ai[0]++;

        }
        


            // Additional hooks/methods here.
        }
}