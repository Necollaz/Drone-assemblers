# Drone Resource Collection System

## Что реализовано
- Автоматический сбор ресурсов дронами на игровой карте.
- Поиск ближайшего свободного ресурса, полёт к нему, сбор (2 с) и исчезновение ресурса.
- Возвращение дрона на базу:
    - Если точка разгрузки свободна — прямая посадка и выгрузка (1 – 2 с) с эффектом.
    - Если занята — переход на одну из свободных точек ожидания и ожидание своего слота.
- Поддержка динамического изменения количества дронов через UI-дропдаун.
- Глобальное изменение скорости всех дронов через ползунок.
- Отображение маршрута дрона в режиме отладки (`DebugToggleUI`).
- Инстансинг статических мешей через `Graphics.DrawMeshInstanced` для оптимизации.

## Архитектура

### Основные модули
1. **BaseComponent**  
   Управляет одной базой:
    - Спавн дронов, точки разгрузки и ожидания
    - Очередь на разгрузку (`UnloadQueueProcessor`)
    - Приём ресурсов и визуальные эффекты (`ResourceReceiver`)
    - Выдача событий для UI (счётчик ресурсов)

2. **DroneSpawner / DroneFactory / DroneManager**
    - `DroneSpawner` создаёт и уничтожает дронов по запросу UI, хранит глобальную скорость
    - `DroneFactory` инстанцирует `DroneView` и `DroneModel`
    - `DroneManager` обновляет всех дронов каждый кадр

3. **DroneController / DroneMovement / DroneView / DroneModel**
    - `DroneController` хранит состояние, управляет жизненным циклом (поиск, сбор, ожидание, разгрузка)
    - `DroneMovement` инкапсулирует логику NavMeshAgent (навигация, автоматическое торможение, остановка)
    - `DroneView` отвечает за визуализацию дрона: материал, рендер пути, корутины сбора/выгрузки
    - `DroneModel` хранит текущее состояние и целевой ресурс

4. **GameResource (Spawner / Pool / Models / UI)**
    - `ResourceSpawner` инициализирует и периодически спавнит ресурсы на NavMesh
    - `ResourcePool` управляет доступными ресурсами
    - `ResourceModel` отвечает за логику существования и сбора ресурса
    - `ResourceCounter` обновляет UI-счетчик на базе

5. **StaticMeshInstancerController**  
   Включён в Bootstrap, инстансит статические объекты (меши) одним draw call-ом для производительности.

6. **UI-модули**
    - `SpeedSliderUI` — ползунок скорости
    - `DroneCountDropdownUI` — дропдаун количества дронов
    - `SpawnIntervalUI` — поля интервала спавна ресурсов
    - `DebugToggleUI` — переключатель отображения путей

### Поток логики
1. **Bootstrap** (`GameBoostrap`)
    - Инициализирует все спавнеры и пулы
    - Подписывает UI-обработчики
    - Запускает `DroneSpawner.UpdateAll()` в `Update()`

2. **Обновление дронов**
    - В `Update()` каждый кадр `DroneSpawner` вызывает `DroneManager.UpdateAll()`
    - `DroneController.UpdateController()`
        - Если Idle — ищет ресурс или резервирует точку ожидания
        - Если собрал ресурс — встаёт в очередь разгрузки
        - Навигация через `DroneMovement`
        - События `Arrived` переключают состояние и запускают корутины сбора/выгрузки

3. **Очередь разгрузки** (`UnloadQueueProcessor`)
    - Контролирует одновременный доступ к точке разгрузки
    - При вызове `NotifyFinished` выпускает следующий дрон из очереди

4. **UI**
    - Изменения ползунка скорости и дропдауна количества дронов сразу передаются в `DroneSpawner`
    - Чекбокс отладки меняет глобальный флаг `DebugSettings.ShowDronePaths`, `DroneView` перерисовывает маршруты

## Использованные инструменты и подходы
- **Unity**: NavMeshAgent, LineRenderer, ScriptableObject, Coroutine
- **NavMesh**: поиск безопасной точки, автоматическое торможение, Warp для точного встраивания
- **Object Pooling**: для ParticleSystem и ResourceModel
- **Instanced Rendering**: `Graphics.DrawMeshInstanced` через `StaticMeshInstancerController`
- **Событийно-подписная архитектура**: события Arrived, GatherComplete, UnloadComplete, ResourceReceived
- **UI-компоненты**: Slider, TMP_Dropdown, Button, Toggle, TextMeshProUGUI  