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
    Frontend->>Backend: POST /join (Inicialização)
    Backend-->>Frontend: Payload: Configs + Tabela de Pagamentos
    Backend-->>Frontend: Payload: Estado do Jogador (Balance, Jackpots)

    %% Aposta
    Frontend->>Backend: POST /bet (bet_value: 0.45, auto_spin: false)
    Backend-->>Frontend: Payload: Resultado do Spin (winning_lines, payout)
    Backend-->>Frontend: Payload: Atualização do Balance (5993.21)
    Backend-->>Frontend: Payload: Atualização dos Jackpots

    %% Bônus (Opcional - Gatilho do Servidor)
    alt Bônus Ativado (Jackpot Completo)
        Backend->>Frontend: Evento: Início do Bônus (Multiplicadores)
        loop Minigame (Mines/Roleta)
            Frontend->>Backend: POST /bonus (Seleção de Posição)
            Backend-->>Frontend: Resultado Parcial (Valor do Multiplicador)
        end
        Backend-->>Frontend: Payload: Resultado Final (bonus_cashout: 580.14)
        Backend-->>Frontend: Atualização do Balance (6019.86)
    end

    %% Cashout
    Frontend->>Backend: POST /cashout
    Backend-->>Frontend: Payload: Resultado (total_cashout: 0.30)
    Backend-->>Frontend: Payload: Atualização do Balance (942326.37)
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
