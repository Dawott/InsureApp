<template>
  <div class="fixed inset-0 bg-gray-500 bg-opacity-75 flex items-center justify-center">
    <div class="bg-white rounded-lg shadow-xl max-w-2xl w-full m-4">
      <!-- Modal Header -->
      <div class="px-6 py-4 border-b border-gray-200">
        <div class="flex justify-between items-center">
          <h2 class="text-xl font-semibold text-gray-900">Edytuj</h2>
          <button @click="$emit('close')" class="text-gray-400 hover:text-gray-500">
            <span class="sr-only">Zamknij</span>
            <svg class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      </div>

      <!-- Form -->
      <form @submit.prevent="handleSubmit" class="px-6 py-4">
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
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

        <!-- Error Message -->
        <div v-if="error" class="mt-4 text-sm text-red-600">
          {{ error }}
        </div>

        <!-- Form Actions -->
        <div class="mt-6 flex justify-end space-x-3">
          <button type="button"
                  @click="$emit('close')"
                  class="bg-white py-2 px-4 border border-gray-300 rounded-md shadow-sm text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500">
            Anuluj
          </button>
          <button type="submit"
                  :disabled="loading"
                  class="inline-flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 disabled:opacity-50">
            <span v-if="loading">Zapisywanie...</span>
            <span v-else>Zapisz zmiany</span>
          </button>
        </div>
      </form>
    </div>
  </div>
</template>

<script>
import { ref } from 'vue';
import { usersService } from '@/services/usersService';

export default {
  name: 'EditUserModal',
  props: {
    user: {
      type: Object,
      required: true
    }
  },
  emits: ['close', 'update'],
  setup(props, { emit }) {
    const formData = ref({
      id: props.user.id,
      firstName: props.user.firstName,
      lastName: props.user.lastName,
      email: props.user.email,
      username: props.user.username,
      phoneNumber: props.user.phoneNumber || '',
      address: props.user.address || '',
      isActive: props.user.isActive,
      passwordHash: props.user.passwordHash
    });

    const loading = ref(false);
    const error = ref(null);

    const handleSubmit = async () => {
      try {
        loading.value = true;
        error.value = null;

        const response = await usersService.updateUser(props.user.id, formData.value);

        if (response.success) {
          emit('update', response.data);
          emit('close');
        } else {
          error.value = response.message;
        }
      } catch (err) {
        error.value = 'Błąd w update klienta.';
        console.error('Błąd:', err);
      } finally {
        loading.value = false;
      }
    };

    return {
      formData,
      loading,
      error,
      handleSubmit
    };
  }
};
</script>
