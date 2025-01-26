<template>
  <div class="max-w-2xl mx-auto">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-2xl font-semibold text-gray-900">Utwórz klienta ręcznie</h1>
      <router-link to="/users"
                   class="text-blue-600 hover:text-blue-800">
        Powrót do klientów
      </router-link>
    </div>

    <form @submit.prevent="handleSubmit" class="space-y-6">
      <!-- User Details Card -->
      <div class="bg-white shadow rounded-lg p-6">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
          <!-- First Name -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Imię</label>
            <input type="text"
                   v-model="formData.firstName"
                   required
                   class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" />
          </div>

          <!-- Last Name -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Nazwisko</label>
            <input type="text"
                   v-model="formData.lastName"
                   required
                   class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" />
          </div>

          <!-- Email -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Email</label>
            <input type="email"
                   v-model="formData.email"
                   required
                   class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" />
          </div>

          <!-- Username -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Username</label>
            <input type="text"
                   v-model="formData.username"
                   required
                   class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" />
          </div>

          <!-- Password -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Hasło</label>
            <input type="password"
                   v-model="formData.password"
                   required
                   class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" />
          </div>

          <!-- Confirm Password -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Potwierdź hasło</label>
            <input type="password"
                   v-model="confirmPassword"
                   required
                   class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" />
          </div>

          <!-- Phone Number -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Telefon</label>
            <input type="tel"
                   v-model="formData.phoneNumber"
                   class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" />
          </div>

          <!-- Address -->
          <div>
            <label class="block text-sm font-medium text-gray-700">Adres</label>
            <input type="text"
                   v-model="formData.address"
                   class="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" />
          </div>

          <!-- Active Status -->
          <div class="col-span-2">
            <div class="flex items-center">
              <input type="checkbox"
                     v-model="formData.isActive"
                     class="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded" />
              <label class="ml-2 block text-sm text-gray-900">Konto aktywne</label>
            </div>
          </div>
        </div>
      </div>

      <!-- Error Message -->
      <div v-if="error" class="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded">
        {{ error }}
      </div>

      <!-- Submit Button -->
      <div class="flex justify-end">
        <button type="submit"
                :disabled="loading || !isFormValid"
                class="bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded-md disabled:opacity-50">
          <span v-if="loading">Tworzenie...</span>
          <span v-else>Tworzenie użytkownika</span>
        </button>
      </div>
    </form>
  </div>
</template>

<script>
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { usersService } from '@/services/usersService';

export default {
  name: 'CreateUser',
  setup() {
    const router = useRouter();
    const loading = ref(false);
    const error = ref(null);
    const confirmPassword = ref('');

    const formData = ref({
      firstName: '',
      lastName: '',
      email: '',
      username: '',
      password: '',
      phoneNumber: '',
      address: '',
      isActive: true
    });

    const isFormValid = computed(() => {
      return formData.value.password &&
             formData.value.password === confirmPassword.value;
    });

    const handleSubmit = async () => {
      if (!isFormValid.value) {
        error.value = 'Hasło się nie zgadza';
        return;
      }

      try {
        loading.value = true;
        error.value = null;

        const response = await usersService.createUser({
          ...formData.value,
          passwordHash: formData.value.password //HASH
        });

        if (response.success) {
          router.push(`/users/${response.data.id}`);
        } else {
          error.value = response.message;
        }
      } catch (err) {
        error.value = 'Błąd podczas tworzenia użytkownika';
        console.error('Błąd:', err);
      } finally {
        loading.value = false;
      }
    };

    return {
      formData,
      confirmPassword,
      loading,
      error,
      isFormValid,
      handleSubmit
    };
  }
};
</script>
