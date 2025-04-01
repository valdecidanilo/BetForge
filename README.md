# BetForge

*Versão 2.0 | Última atualização: [DATA]*

---

## Índice
1. [Visão Geral](#visão-geral)
2. [Estrutura de Branches](#estrutura-de-branches)
3. [Organização de Pastas](#organização-de-pastas)

---

## Visão Geral <a name="visão-geral"></a>
Este documento define a estruturação de um projeto Unity usando Git para:
- Evitar sobrescritas de cenas/sprites.
- Isolar o código central (`core`).
- Facilitar colaboração em múltiplas branches.

---

## Estrutura de Branches <a name="estrutura-de-branches"></a>

### Principais Branches
| Nome          | Função                                                                 |
|---------------|------------------------------------------------------------------------|
| **`master`**    | Versão estável (implantável para o público).                          |
| **`core`**    | Sistemas centrais (save, input, UI base).                             |
| **`develop`** | Branch de integração para features.                                   |
| **`feature/*`** | Branches temporárias para funcionalidades específicas (ex: `feature/new-menu`). |

### Diagrama de Fluxo
```mermaid
graph TD
    A[core] --> B[main]
    B --> C[develop]
    C --> D[feature/new-menu]
    C --> E[feature/enemy-ai]
    D --> C
    E --> C
    C -->|Release| B
```

### Diagrama Teste
```mermaid
sequenceDiagram
    participant Frontend
    participant Backend
    
    %% Entrada
    Frontend->>Backend: POST /join
    Backend-->>Frontend: Pay Table + Configs
    
    %% Aposta
    Frontend->>Backend: POST /bet (Valor: 4, Minas: 1)
    Backend-->>Frontend: Confirmação (Floor 0)
    
    %% Gameplay
    loop Cada jogada
        Frontend->>Backend: POST /reveal (Posição)
        alt Acerta
            Backend-->>Frontend: Atualiza (Multiplicador + Floor)
        else Erra
            Backend-->>Frontend: Game Over (Bombas)
        end
    end
    
    %% Cashout
    Frontend->>Backend: POST /cashout
    Backend-->>Frontend: Resultado (1.1085x)
    Backend-->>Frontend: Balance (942326.37)
```

## Organização de Pastas <a name="organização-de-pastas"></a>
```
Assets/
├─ Core/                  # Sistemas compartilhados
│  ├─ Scripts/
│  ├─ Data/
│  ├─ Prefabs/            # Prefabs base (ex: Player.prefab)
├─ Features/              # Funcionalidades isoladas
│  ├─ NewMenu/            # Implementação completa de um menu
│  │  ├─ Scenes/          # Cenas específicas (Menu.unity)
│  │  ├─ Sprites/         # Assets visuais
├─ Scenes/
│  ├─ Main.unity          # Cena inicial (gerencia carregamento)
ProjectSettings/          # Configurações do projeto (só altere na branch `core`)
```
