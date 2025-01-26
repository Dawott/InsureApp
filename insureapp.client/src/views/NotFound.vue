<template>
  <div class="min-h-screen flex items-center justify-center">
    <div class="text-center">
      <h1 class="text-6xl font-bold text-gray-900 mb-4">404</h1>
      <p class="text-xl text-gray-600 mb-8">Nie ma takiej strony!</p>
      <router-link to="/"
                   class="bg-blue-600 text-white px-6 py-3 rounded-md hover:bg-blue-700 inline-block">
        Wróć do głównego widoku
      </router-link>
    </div>
  </div>
</template>

<script>
export default {
  name: 'NotFound'
}
</script>
