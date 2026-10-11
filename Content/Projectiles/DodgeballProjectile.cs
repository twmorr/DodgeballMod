using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DodgeballMod.Content.Projectiles
{
    public class DodgeballProjectile : ModProjectile
    {
        // Top speed the ball steers toward, both when homing and when returning.
        private const float HomingSpeed = 12f;

        // How strongly the ball turns toward its goal each tick (0 = never, 1 = instantly).
        private const float TurnStrength = 0.08f;

        public override string Texture => $"Terraria/Images/Item_5543";
        //public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.BeachBall}";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType = DamageClass.Ranged;

            Projectile.timeLeft = 300;
            Projectile.penetrate = -1; // Infinite penetration, so it can hit multiple enemies and doesnt despawn after hitting one.

            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            // ai[0] stores which behavior the dodgeball is currently using.
            // 0 = homing toward enemies
            // 1 = returning to the player
            bool isReturning = Projectile.ai[0] == 1f;

            // Derive tile collision from ai[0] every tick so it stays in sync in multiplayer.
            // Returning balls ignore tiles so they can fly back through walls.
            Projectile.tileCollide = !isReturning;

            if (isReturning)
            {
                Player owner = Main.player[Projectile.owner];

                // If the owner is gone or dead, there's nothing to return to.
                if (!owner.active || owner.dead)
                {
                    Projectile.Kill();
                    return;
                }

                // Once the ball touches the player, remove it.
                if (Projectile.Hitbox.Intersects(owner.Hitbox))
                {
                    Projectile.Kill();
                    return;
                }

                // Get a direction pointing from the ball back to the player.
                Vector2 directionToPlayer = Projectile.DirectionTo(owner.Center);

                Vector2 returnVelocity = directionToPlayer * HomingSpeed;

                // Use the same smooth steering we used for enemy homing.
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, returnVelocity, TurnStrength);

                return;
            }

            NPC target = FindClosestEnemy(500f);

            // No enemy nearby. Just continue moving normally.
            if (target == null)
                return;

            // Get a direction pointing from the ball to the enemy.
            Vector2 directionToTarget = Projectile.DirectionTo(target.Center);

            // This is the velocity we'd ideally like to have.
            Vector2 desiredVelocity = directionToTarget * HomingSpeed;

            // Gradually turn our current velocity toward it.
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, TurnStrength);
        }

        private NPC FindClosestEnemy(float maxDistance)
        {
            // Start with no target selected.
            NPC closestEnemy = null;

            // Any enemy farther away than this will be ignored.
            float closestDistance = maxDistance;

            // Check every NPC slot in the world.
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];

                // Skip NPCs that shouldn't be targeted by homing projectiles.
                if (!npc.CanBeChasedBy())
                    continue;

                // Measure the distance from the dodgeball to this NPC.
                float distance = Vector2.Distance(Projectile.Center, npc.Center);

                // If this NPC is closer than our current best target,
                // remember it as the new closest enemy.
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = npc;
                }
            }

            // Returns the closest valid enemy, or null if none were in range.
            return closestEnemy;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // When the dodgeball hits an enemy, switch to returning to the player.
            Projectile.ai[0] = 1f;

            // Reset the lifetime so the ball doesn't vanish partway through its return trip.
            Projectile.timeLeft = 300;

            Player owner = Main.player[Projectile.owner];

            // DirectionTo uses SafeNormalize, so this can't become NaN if the ball is on top of the player.
            Vector2 directionToPlayer = Projectile.DirectionTo(owner.Center);

            // Give the ball an immediate, strong bounce away from the enemy.
            Projectile.velocity = directionToPlayer * 30f;

            Projectile.netUpdate = true; // Sync the projectile's state with other clients in multiplayer.
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.ai[0] == 1f)
            {
                // If the dodgeball is returning to the player, it shouldn't hit enemies.
                return false;
            }

            // Otherwise use vanilla's rules (this also lets the ball hit target dummies).
            return null;
        }
    }
}