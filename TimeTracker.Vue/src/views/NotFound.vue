<template>
  <el-empty
    :image-size="200"
    description="Looks like the page your are looking for does not exist"
  />
</template>

<script lang="ts">
export default {
  data() {
    return {}
  },

  methods: {
    gotoIndex() {
      window.location.replace('/timetracker')
    },
  },
  mounted() {
    this.gotoIndex()
  },
}
</script>

<style scoped></style>
