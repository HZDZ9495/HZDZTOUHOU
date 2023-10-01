using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace HZDZTOUHOU
{
    public class reimu1 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("reimu's projectile");
        }

        public override void SetDefaults()
        {
            Projectile.arrow = false;
            Projectile.width = 14;
            Projectile.height = 12;
            Projectile.aiStyle = 1; // or 1
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            Projectile.hostile = true;
            Projectile.maxPenetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.spriteDirection = Projectile.direction;

        }
        
        
        
        // Additional hooks/methods here.
    }
}