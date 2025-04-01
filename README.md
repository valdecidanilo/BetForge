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
    
    %% Entrada no Jogo
    Frontend->>Backend: POST /join (Entrar no jogo)
    Backend-->>Frontend: Configurações + Pay Table
    Note right of Backend: {"pay_table": [{"mines":1,"odds":[1.0104,...]},...]}
    
    %% Aposta Inicial
    Frontend->>Backend: POST /bet 
    Note left of Frontend: {"bet_value":4,"number_of_mines":3}
    Backend-->>Frontend: Confirmação da Aposta
    Note right of Backend: {"status":0,"tiles":[0,0,...],"payout_multiplier_on_next":1.1022}
    
    %% Loop de Jogadas
    loop Até Cashout ou Mina
        Frontend->>Backend: POST /reveal (Clicar em tile)
        Backend-->>Frontend: Atualização do Estado
        Note right of Backend: Atualiza "tiles" e "payout_multiplier"
        
        alt Tile seguro
            Backend-->>Frontend: Multiplicador atualizado
            Note right of Backend: "payout_multiplier_on_next": novo_valor
        else Mina encontrada
            Backend-->>Frontend: Fim de jogo (perda)
            Note right of Backend: "status":2, tiles revelados
        end
    end
    
    %% Cashout
    Frontend->>Backend: POST /cashout
    Backend-->>Frontend: Resultado do Cashout
    Note right of Backend: {"payout_multiplier":1.1022,"status":1,"tiles":[...]}
    Backend-->>Frontend: Atualização de Saldo
    Note right of Backend: {"balance":"5999.03","playing_bets":[]}
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
