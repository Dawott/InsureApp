<script>
  export default {
    name: 'App'
  }
</script>

<template>
  <div class="flex h-screen max-w-screen-xl bg-gray-100">
    <!-- Sidebar -->
    <aside class="w-56 flex-shrink-0 bg-gray-800 text-white">
      <div class="p-4">
        <h1 class="text-xl font-bold">InsureApp</h1>
      </div>

      <!-- Nawigator -->
      <nav class="mt-4">
        <!-- SEKCJA RAPORTY -->
        <div class="px-4 py-2">
          <h2 class="text-xs uppercase tracking-wide text-gray-400">Zgłoszenia szkód</h2>
          <router-link to="/reports" class="block px-4 py-2 mt-2 text-sm hover:bg-gray-700 rounded">
            Wszystkie zgłoszenia
          </router-link>
          <router-link to="/reports/new" class="block px-4 py-2 mt-1 text-sm hover:bg-gray-700 rounded">
            Utwórz zgłoszenie
          </router-link>
        </div>

        <!-- ZARZĄDZANIE KLIENTAMI -->
        <div class="px-4 py-2 mt-4">
          <h2 class="text-xs uppercase tracking-wide text-gray-400">Zarządzanie Klientami</h2>
          <router-link to="/users" class="block px-4 py-2 mt-2 text-sm hover:bg-gray-700 rounded">
            Wszyscy klienci
          </router-link>
          <router-link to="/users/new" class="block px-4 py-2 mt-1 text-sm hover:bg-gray-700 rounded">
            Dodaj klienta ręcznie
          </router-link>
        </div>

        <!-- ZARZĄDZANIE AGENTAMI -->
        <div class="px-4 py-2 mt-4">
          <h2 class="text-xs uppercase tracking-wide text-gray-400">Agenci ubezpieczeniowi</h2>
          <router-link to="/agents" class="block px-4 py-2 mt-2 text-sm hover:bg-gray-700 rounded">
            Wszyscy agenci
          </router-link>
          <router-link to="/agents/new" class="block px-4 py-2 mt-1 text-sm hover:bg-gray-700 rounded">
            Dodaj agenta ręcznie
          </router-link>
        </div>

        <!-- TYPY UBEZPIECZEń -->
        <div class="px-4 py-2 mt-4">
          <h2 class="text-xs uppercase tracking-wide text-gray-400">Typy ubezpieczeń</h2>
          <router-link to="/insurance-types" class="block px-4 py-2 mt-2 text-sm hover:bg-gray-700 rounded">
            Zarządzanie typami
          </router-link>
        </div>
      </nav>
    </aside>

    <!-- Content -->
    <main class="flex-1 flex flex-col min-w-0">
      <!-- TOP BAR -->
      <div class="bg-white shadow-sm">
        <div class="px-6 py-4">
          <h1 class="text-xl font-semibold text-gray-800">Dashboard</h1>
        </div>
      </div>

      <!-- Strona z router -->
      <div class="flex-1 p-6 overflow-x-hidden">
        <div class="max-w-7xl mx-auto w-full">
          <router-view></router-view>
        </div>
        </div>
</main>
  </div>
</template>
