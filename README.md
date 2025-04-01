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
    participant Jogador
    participant Frontend
    participant Backend
    
    %% Entrada no Jogo
    Jogador->>Frontend: Clica para entrar no jogo
    Frontend->>Backend: POST /join
    Backend-->>Frontend: Configurações + Pay Table
    Frontend-->>Jogador: Exibe interface do jogo
    
    %% Aposta Inicial
    Jogador->>Frontend: Define aposta (valor=4, minas=1)
    Frontend->>Backend: POST /bet
    Note left of Frontend: {"bet_value":4,"number_of_mines":1}
    Backend-->>Frontend: Confirmação da aposta
    Note right of Backend: {"status":0,"payout_multiplier_on_next":1.1085,...}
    Frontend-->>Jogador: Exibe grid vazio (floor 0)
    
    %% Loop de Jogadas
    loop Até Cashout ou Explosão
        Jogador->>Frontend: Clica em célula (posição 4)
        Frontend->>Backend: POST /reveal
        Note left of Frontend: {"position":4}
        
        alt Célula segura
            Backend-->>Frontend: Atualização do jogo
            Note right of Backend: {"floor":1,"payout_multiplier":1.1085,...}
            Frontend-->>Jogador: Atualiza grid e multiplicador
            
            alt Ativação de bônus
                Backend-->>Frontend: special_round=true
                Frontend-->>Jogador: Exibe modo especial
            end
            
        else Célula com bomba
            Backend-->>Frontend: Fim de jogo
            Note right of Backend: {"bomb_hit":true,"status":2}
            Frontend-->>Jogador: Mostra explosão
        end
    end
    
    %% Cashout
    Jogador->>Frontend: Clica em Cashout
    Frontend->>Backend: POST /cashout
    Backend-->>Frontend: Resultado do cashout
    Note right of Backend: {"payout_multiplier":1.1085,"total_cashout":4.43,...}
    Backend-->>Frontend: Atualização de saldo
    Note right of Backend: {"balance":942326.37}
    Frontend-->>Jogador: Exibe ganhos e novo saldo
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
