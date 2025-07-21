#pragma once

class MyClass
{
public:
    MyClass() = default;

    // [CSharpAPI]
    int Add(int a, int b) const;
};

// [CSharpAPI]
int Add(int a, int b);