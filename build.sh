#!/bin/bash

# 7D2D Basic ESP Framework - Git Bash Script
# Usage: ./build.sh [command]

set -e

PROJECT_NAME="7D2D Basic ESP Framework"
DLL_NAME="SevenDtDAibot.dll"
BUILD_DIR="bin/Release"
SOURCE_DIR="."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Show help
show_help() {
    echo "7D2D Basic ESP Framework - Build Script"
    echo ""
    echo "Usage: $0 [command]"
    echo ""
    echo "Commands:"
    echo "  build       Build the project in Release mode"
    echo "  clean       Clean build artifacts"
    echo "  rebuild     Clean and rebuild the project"
    echo "  run         Build and copy DLL to game directory"
    echo "  status      Show git status and build info"
    echo "  help        Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0 build    # Build the project"
    echo "  $0 clean    # Clean build artifacts"
    echo "  $0 run      # Build and prepare for injection"
}

# Check if dotnet is available
check_dotnet() {
    if ! command -v dotnet &> /dev/null; then
        print_error "dotnet CLI not found. Please install .NET SDK."
        exit 1
    fi
}

# Clean build artifacts
clean_build() {
    print_status "Cleaning build artifacts..."
    
    if [ -d "bin" ]; then
        rm -rf bin
        print_success "Removed bin directory"
    fi
    
    if [ -d "obj" ]; then
        rm -rf obj
        print_success "Removed obj directory"
    fi
    
    print_success "Build artifacts cleaned"
}

# Build the project
build_project() {
    print_status "Building $PROJECT_NAME..."
    
    check_dotnet
    
    # Build in Release mode
    dotnet build 7D2D.csproj --configuration Release --verbosity minimal
    
    if [ $? -eq 0 ]; then
        print_success "Build completed successfully"
        
        # Check if DLL exists
        if [ -f "$BUILD_DIR/$DLL_NAME" ]; then
            local dll_size=$(stat -c%s "$BUILD_DIR/$DLL_NAME" 2>/dev/null || stat -f%z "$BUILD_DIR/$DLL_NAME" 2>/dev/null)
            print_success "DLL created: $BUILD_DIR/$DLL_NAME ($(echo $dll_size | numfmt --to=iec)) bytes"
        else
            print_error "DLL not found at expected location: $BUILD_DIR/$DLL_NAME"
            exit 1
        fi
    else
        print_error "Build failed"
        exit 1
    fi
}

# Rebuild project
rebuild_project() {
    print_status "Rebuilding $PROJECT_NAME..."
    clean_build
    build_project
}

# Prepare for injection
prepare_run() {
    print_status "Preparing for game injection..."
    
    build_project
    
    if [ -f "$BUILD_DIR/$DLL_NAME" ]; then
        print_success "Ready for injection!"
        print_status "DLL location: $BUILD_DIR/$DLL_NAME"
        print_status "Injection settings:"
        print_status "  Namespace: SevenDtDAibot"
        print_status "  Class: Loader"
        print_status "  Method: init"
    else
        print_error "DLL not found. Build failed."
        exit 1
    fi
}

# Show project status
show_status() {
    print_status "$PROJECT_NAME Status"
    echo ""
    
    # Git status
    if [ -d ".git" ]; then
        print_status "Git Status:"
        git status --porcelain
        echo ""
        
        print_status "Git Branch:"
        git branch --show-current
        echo ""
        
        print_status "Last Commit:"
        git log -1 --oneline
        echo ""
    else
        print_warning "Not a git repository"
    fi
    
    # Build status
    if [ -f "$BUILD_DIR/$DLL_NAME" ]; then
        local dll_size=$(stat -c%s "$BUILD_DIR/$DLL_NAME" 2>/dev/null || stat -f%z "$BUILD_DIR/$DLL_NAME" 2>/dev/null)
        local build_time=$(stat -c%y "$BUILD_DIR/$DLL_NAME" 2>/dev/null || stat -f%Sm "$BUILD_DIR/$DLL_NAME" 2>/dev/null)
        print_success "DLL Status: Built ($(echo $dll_size | numfmt --to=iec)) bytes"
        print_status "Last Build: $build_time"
    else
        print_warning "DLL not built"
    fi
    
    # Project files
    echo ""
    print_status "Project Files:"
    find . -name "*.cs" -not -path "./bin/*" -not -path "./obj/*" | sort
}

# Main script logic
case "${1:-help}" in
    "build")
        build_project
        ;;
    "clean")
        clean_build
        ;;
    "rebuild")
        rebuild_project
        ;;
    "run")
        prepare_run
        ;;
    "status")
        show_status
        ;;
    "help"|*)
        show_help
        ;;
esac
