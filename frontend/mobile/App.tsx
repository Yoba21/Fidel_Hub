import React from 'react';
import { StyleSheet, Text, View, SafeAreaView } from 'react-native';
import { StatusBar } from 'expo-status-bar';

export default function App() {
  return (
    <SafeAreaView style={styles.container}>
      <StatusBar style="light" />
      <View style={styles.card}>
        <Text style={styles.title}>Fidel Hub (ፊ)</Text>
        <Text style={styles.subtitle}>Offline-First School Mobile Client</Text>
        <Text style={styles.description}>
          Equipped with Expo SQLite local cache and sync engine for teachers, parents, and students.
        </Text>
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#FAF7F0',
    alignItems: 'center',
    justifyContent: 'center',
    padding: 20,
  },
  card: {
    backgroundColor: '#FFFFFF',
    borderRadius: 16,
    padding: 24,
    borderWidth: 1,
    borderColor: '#E5E7EB',
    alignItems: 'center',
    width: '100%',
    maxWidth: 380,
  },
  title: {
    fontSize: 26,
    fontWeight: '700',
    color: '#1B5E3F',
    marginBottom: 8,
  },
  subtitle: {
    fontSize: 16,
    fontWeight: '600',
    color: '#E0A526',
    marginBottom: 12,
  },
  description: {
    fontSize: 14,
    color: '#1F2933',
    textAlign: 'center',
    lineHeight: 20,
  },
});
