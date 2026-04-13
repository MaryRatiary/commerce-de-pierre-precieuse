import React, { useState, useEffect } from 'react';
import { productsAPI } from '../../services/api';
import ProductCard from '../../components/ProductCard';

const Home = () => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [email, setEmail] = useState('');
  const [subscribed, setSubscribed] = useState(false);

  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await productsAPI.getAll();
        setProducts(response.data);
        setLoading(false);
      } catch (err) {
        setError('Erreur lors du chargement des produits');
        setLoading(false);
      }
    };

    fetchProducts();
  }, []);

  const handleNewsletterSubmit = (e) => {
    e.preventDefault();
    if (email) {
      setSubscribed(true);
      setEmail('');
      setTimeout(() => setSubscribed(false), 3000);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div style={{ textAlign: 'center' }}>
          <div className="loading-spinner" style={{ margin: '0 auto 1rem' }}></div>
          <p style={{ color: 'var(--color-text-light)', fontSize: '1.125rem' }}>Chargement des merveilles...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container">
        <div className="error-message">{error}</div>
      </div>
    );
  }

  return (
    <div style={{ background: 'var(--color-bg)' }}>
      {/* Hero Section */}
      <section className="hero-section">
        <div className="container" style={{ width: '100%', maxWidth: '1280px', margin: '0 auto' }}>
          <div style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))',
            gap: '2rem',
            alignItems: 'center'
          }}>
            <div className="hero-content">
              <h1 className="hero-title">
                Découvrez les Trésors de Madagascar
              </h1>
              <p className="hero-subtitle">
                Des pierres précieuses et des bijoux artisanaux authentiques directement de Madagascar. Chaque pièce raconte une histoire d'excellence et de tradition.
              </p>
              <div className="hero-button">
                <button className="btn btn-primary" style={{ fontSize: '1.125rem' }}>
                  Explorez notre Collection
                </button>
              </div>
            </div>
            <div className="hero-image">
              <img 
                src="/collection amethyste.jpeg" 
                alt="Collections de pierres précieuses"
                style={{ width: '100%', height: 'auto' }}
              />
            </div>
          </div>
        </div>
      </section>

      {/* Featured Categories Section */}
      <section style={{ padding: '5rem 1.25rem' }}>
        <div className="container">
          <h2 className="section-title">Nos Catégories Phares</h2>
          
          <div style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))',
            gap: '1.5rem'
          }}>
            {[
              {
                title: 'Pierres Précieuses',
                icon: '💎',
                description: 'Saphirs, rubis et émeraudes authentiques'
              },
              {
                title: 'Bijoux Artisanaux',
                icon: '✨',
                description: 'Créations exclusives faites à la main'
              },
              {
                title: 'Collections Exclusives',
                icon: '👑',
                description: 'Éditions limitées et pièces uniques'
              },
              {
                title: 'Nouveautés',
                icon: '🌟',
                description: 'Dernières découvertes et tendances'
              }
            ].map((category, index) => (
              <div key={index} className="elegant-card card" style={{ cursor: 'pointer', padding: '2rem' }}>
                <div style={{ display: 'flex', flexDirection: 'column', justifyContent: 'space-between', height: '100%' }}>
                  <div>
                    <div style={{
                      fontSize: '3.5rem',
                      marginBottom: '1rem',
                      transition: 'transform 0.3s ease',
                    }} 
                    onMouseEnter={(e) => e.currentTarget.style.transform = 'scale(1.2)'}
                    onMouseLeave={(e) => e.currentTarget.style.transform = 'scale(1)'}
                    >
                      {category.icon}
                    </div>
                    <h3 style={{ fontSize: '1.25rem', fontWeight: '700', marginBottom: '0.5rem', color: 'var(--color-text-dark)' }}>
                      {category.title}
                    </h3>
                    <p style={{ color: 'var(--color-text-light)', marginBottom: '1.5rem' }}>
                      {category.description}
                    </p>
                  </div>
                  <button className="btn btn-secondary" style={{ width: '100%' }}>
                    Explorer →
                  </button>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Special Offers Section */}
      <section style={{
        padding: '5rem 1.25rem',
        background: 'linear-gradient(to bottom right, #fafaf8, var(--color-white), #fafaf8)'
      }}>
        <div className="container">
          <h2 className="section-title">Offres Spéciales</h2>
          
          <div style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(280px, 1fr))',
            gap: '2rem'
          }}>
            <div className="elegant-card card" style={{ overflow: 'hidden' }}>
              <div style={{ padding: '3rem', textAlign: 'center', position: 'relative', zIndex: 10 }}>
                <div style={{ fontSize: '3.5rem', marginBottom: '1rem' }}>🚚</div>
                <h3 style={{ fontSize: '1.5rem', fontWeight: '700', marginBottom: '0.75rem', color: 'var(--color-text-dark)' }}>
                  Livraison Gratuite
                </h3>
                <p style={{ color: 'var(--color-text-light)', fontSize: '1.125rem' }}>
                  Sur toutes les commandes internationales, partout dans le monde
                </p>
              </div>
            </div>
            
            <div className="elegant-card card" style={{
              overflow: 'hidden',
              background: 'linear-gradient(135deg, var(--color-secondary-medium), var(--color-secondary-dark))'
            }}>
              <div style={{ padding: '3rem', textAlign: 'center', position: 'relative', zIndex: 10 }}>
                <div style={{ fontSize: '3.5rem', marginBottom: '1rem' }}>🎁</div>
                <h3 style={{ fontSize: '1.5rem', fontWeight: '700', marginBottom: '0.75rem', color: 'var(--color-white)' }}>
                  -20% Bijoux Artisanaux
                </h3>
                <p style={{ color: 'rgba(255, 255, 255, 0.8)', fontSize: '1.125rem' }}>
                  Offre limitée cette semaine uniquement
                </p>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Why Choose Us Section */}
      <section style={{ padding: '5rem 1.25rem' }}>
        <div className="container">
          <h2 className="section-title">Pourquoi Nous Choisir</h2>
          
          <div style={{
            display: 'grid',
            gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))',
            gap: '1.5rem'
          }}>
            {[
              {
                title: 'Authenticité Garantie',
                description: 'Tous nos produits sont certifiés authentiques avec documentation complète',
                icon: '✓'
              },
              {
                title: 'Artisanat Local',
                description: 'Soutien direct aux artisans malgaches et aux communautés locales',
                icon: '🤝'
              },
              {
                title: 'Service Premium',
                description: 'Une équipe dédiée à votre satisfaction 24/7',
                icon: '💬'
              },
              {
                title: 'Livraison Sécurisée',
                description: 'Suivi en temps réel et assurance complète de votre commande',
                icon: '🔒'
              }
            ].map((item, index) => (
              <div key={index} className="elegant-card card" style={{ textAlign: 'center', padding: '2rem' }}>
                <div style={{ fontSize: '2.25rem', marginBottom: '1rem', display: 'inline-block' }}>{item.icon}</div>
                <h3 style={{ fontSize: '1.125rem', fontWeight: '700', marginBottom: '0.75rem', color: 'var(--color-text-dark)' }}>
                  {item.title}
                </h3>
                <p style={{ color: 'var(--color-text-light)', fontSize: '0.875rem', lineHeight: '1.5' }}>
                  {item.description}
                </p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Newsletter Section */}
      <section className="newsletter-section">
        <div className="newsletter-content">
          <h2 className="newsletter-title">Restez Connecté</h2>
          <p className="newsletter-description">
            Inscrivez-vous à notre newsletter pour recevoir nos dernières offres, 
            nouveautés exclusives et conseils en gemologie directement dans votre boîte mail.
          </p>
          
          <form onSubmit={handleNewsletterSubmit} className="newsletter-form">
            <input
              type="email"
              placeholder="Entrez votre email..."
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              className="newsletter-input"
              required
            />
            <button type="submit" className="btn btn-primary" style={{ whiteSpace: 'nowrap' }}>
              S'inscrire
            </button>
          </form>
          
          {subscribed && (
            <p style={{
              marginTop: '1rem',
              color: '#3c3',
              fontWeight: '600',
              animation: 'pulse 2s infinite'
            }}>
              ✓ Merci ! Vérifiez votre email de confirmation.
            </p>
          )}
        </div>
      </section>

      {/* Products Section */}
      <section style={{ padding: '5rem 1.25rem' }}>
        <div className="container">
          <h2 className="section-title">Nos Pierres Précieuses</h2>
          
          {products.length > 0 ? (
            <div style={{
              display: 'grid',
              gridTemplateColumns: 'repeat(auto-fill, minmax(240px, 1fr))',
              gap: '1.5rem'
            }}>
              {products.map((product) => (
                <div key={product.id} className="product-card">
                  <ProductCard product={product} />
                </div>
              ))}
            </div>
          ) : (
            <div style={{ textAlign: 'center', padding: '3rem 0' }}>
              <p style={{ color: 'var(--color-text-light)', fontSize: '1.125rem' }}>Aucun produit disponible pour le moment</p>
            </div>
          )}
        </div>
      </section>

      {/* Call to Action Section */}
      <section style={{
        padding: '5rem 1.25rem',
        background: 'linear-gradient(135deg, var(--color-secondary-medium), var(--color-secondary-dark))'
      }}>
        <div className="container" style={{ maxWidth: '900px' }}>
          <div style={{ textAlign: 'center', color: 'var(--color-white)' }}>
            <h2 style={{ fontSize: '2.25rem', fontWeight: '700', marginBottom: '1.5rem' }}>Prêt à Découvrir la Magie?</h2>
            <p style={{ fontSize: '1.125rem', marginBottom: '2rem', color: 'rgba(255, 255, 255, 0.9)' }}>
              Explorez notre collection complète et trouvez la pièce parfaite qui raconte votre histoire.
            </p>
            <button className="btn" style={{
              background: 'var(--color-white)',
              color: 'var(--color-secondary-medium)',
              fontWeight: '700',
              padding: '0.75rem 2rem',
              transition: 'all 0.3s ease',
              transform: 'scale(1)'
            }}
            onMouseEnter={(e) => {
              e.currentTarget.style.background = '#f5f5f5';
              e.currentTarget.style.transform = 'scale(1.05)';
            }}
            onMouseLeave={(e) => {
              e.currentTarget.style.background = 'var(--color-white)';
              e.currentTarget.style.transform = 'scale(1)';
            }}>
              Parcourir la Collection
            </button>
          </div>
        </div>
      </section>
    </div>
  );
};

export default Home;
