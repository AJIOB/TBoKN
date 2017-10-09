#pragma once
#include <deque>

std::deque<bool> EncodeMessage(std::string message);
std::string DecodeMessage(std::deque<bool> message, int& numOfFoundErrors);
