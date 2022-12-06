<script>
    import HeaderBar from "./HeaderBar.svelte";
    import Sensor from "./Sensor.svelte";
    import TemperatureGraph from "./TemperatureGraph.svelte";
    import TemperatureMap from "./TemperatureMap.svelte";
    import { onMount } from 'svelte';
    import { fetchInitialData, lastReading, readings, onDataChanged } from '../data.js';
    import { temperatureCssFilter } from '../colors.js';

    let icu = {};

    onMount(async () => {
        icu = await fetchInitialData();

        onDataChanged(newICU => {
            icu = newICU;
        });
    });
</script>

<HeaderBar icu={ icu } />

<main>
    <div class="content">
        {#if icu.name}
            <div class="icu-overview">
                <TemperatureMap icu={ icu } />
                <div class="temperature-value" style={temperatureCssFilter(icu, 'temp-avg')}>{ Math.round(lastReading(icu, 'temp-avg')) }</div>
                <div class="hr-value">156</div>
                <div class="spo2-value">97</div>
                <div class="rr-value">116</div>
            </div>

            <h2>Incubator Temperature</h2>
            {#each Object.values(icu?.sensors || {}) as sensor}
                {#if sensor.sensorId.startsWith('moisture-plant-')}
                    <Sensor icu={ icu } sensor={sensor} />
                {/if}
            {/each}
        {/if}
    </div>
</main>

<style>
    h2 {
        margin: 3rem 0 1rem 0;
        font-weight: 300;
    }

    .icu-overview {
        display: flex;
        gap: 1rem;
        align-items: stretch;
    }

    .temperature-value, .humidity-value, .hr-value, .spo2-value, .rr-value {
        min-width: 2rem;
        position: relative;
    }

    .temperature-value, .humidity-value, .hr-value, .spo2-value, .rr-value {
        width: 9rem;
        height: 9rem;
        flex-grow: 0;
        line-height: 5rem;
        text-align: center;
        font-size: 3rem;
        color: white;
    }

    .temperature-value::after {
        content: '°F';
        position: absolute;
        font-size: 1.5rem;
        top: -0.3rem;
    }

    .temperature-value::before, .humidity-value::before, .hr-value::before, .spo2-value::before, .rr-value::before {
        position: absolute;
        font-size: 1.5rem;
        left: 0; right: 0;
        bottom: 1rem;
        height: 1rem;
        line-height: 1rem;
        font-weight: 300;
    }

    .temperature-value::before {
        content: 'avg Temp';
    }

    .humidity-value::before {
        content: 'humidity';
    }

    .humidity-value::after {
        content: '%';
        position: absolute;
        font-size: 1rem;
        top: -0.3rem;
    }

    .hr-value::before {
        content: 'avg HR';
    }

    .spo2-value::before {
        content: 'avg SpO₂';
    }

    .rr-value::before {
        content: 'avg RR';
    }

    .temperature-value {
        background-color: green;
    }

    .humidity-value {
        background-color: #666;
    }

    .hr-value {
        background-color: #444;
    }

    .spo2-value {
        background-color: #555;
    }

    .rr-value {
        background-color: #666;
    } 
</style>
