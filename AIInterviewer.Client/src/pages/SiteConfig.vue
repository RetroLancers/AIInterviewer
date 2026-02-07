<template>
  <div class="site-config-page min-h-screen bg-gray-50 dark:bg-gray-900 py-8">
    <div class="container mx-auto px-4">
      <!-- Page Header -->
      <div class="mb-8">
        <h1 class="text-3xl font-bold text-gray-900 dark:text-gray-100">Site Configuration</h1>
        <p class="mt-2 text-gray-600 dark:text-gray-400">
          Manage your site's global configuration settings, including AI service selection and default voice.
        </p>

      </div>

      <!-- Site Config Editor Component -->
      <SiteConfigEditor />
    </div>
  </div>
</template>

<script setup lang="ts">
import SiteConfigEditor from '@/components/SiteConfigEditor.vue'
</script>
